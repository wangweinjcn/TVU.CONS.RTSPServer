using Media.Rtsp;
using Media.Rtsp.Server.MediaTypes;
using System;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace RTSPServerCreate
{
    public partial class Form1 : Form
    {
        RtspServer _server = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnServerCreate_Click(object sender, EventArgs e)
        {
            string ipAddress = GetLocalIPAddress();

            try
            {
                _server = new RtspServer(IPAddress.Parse(ipAddress), 554);

                #region  Create media source
                //MJPEGMedia source1 = new MJPEGMedia("myLocalStream", @"C:/VTT/Video/HIKARI.mp4");

                RtspSource source2 = new RtspSource("myStream", "rtsp://184.72.239.149/vod/mp4:BigBuckBunny_115k.mov");

                //RFC2435Media source3 = new RFC2435Media("streamPics", @"C:\VTT\ImagesTest\") { Loop = true };

                #endregion

                //Add media source to server
                //_server.TryAddMedia(source1);
                _server.TryAddMedia(source2);
                //_server.TryAddMedia(source3);
                _server.Start();

                txbServerStatus.Text = "Server on";

                txbConnMax.Text = _server.MaximumConnections.ToString();
            }
            catch(Exception ex)
            {
                txbServerStatus.Text = ex.Message;
            }
        }

        private void btnServerStop_Click(object sender, EventArgs e)
        {
            _server.Stop();
            _server.Dispose();
            if (_server.IsDisposed)
            {
                txbServerStatus.Text = "Server is shut down.";
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            TestRtspServer();
        }

        static void TestRtspServer()
        {
            string ipAddress = GetLocalIPAddress();
            try
            {
                using (Media.Rtsp.RtspServer server = new Media.Rtsp.RtspServer(IPAddress.Parse(ipAddress), 554)
                {
                    Logger = new Media.Rtsp.Server.RtspServerConsoleLogger(),
                    ClientSessionLogger = new Media.Rtsp.Server.RtspServerDebugLogger()
                })
                {
                    RFC2435Media sourceScreenCapture = new RFC2435Media("screenCapture", null, false, 1920, 1080, false);

                    server.TryAddMedia(sourceScreenCapture);

                    Thread captureThread = new Thread(new ParameterizedThreadStart((o) =>
                    {

                        Start:

                        using (var bmpScreenShot = new System.Drawing.Bitmap(
                            Screen.PrimaryScreen.Bounds.Width,
                            Screen.PrimaryScreen.Bounds.Height,
                            System.Drawing.Imaging.PixelFormat.Format32bppArgb))
                        {
                            using (var gfxScreenShot = Graphics.FromImage(bmpScreenShot))
                            {
                                while (server.IsRunning)
                                {
                                    try
                                    {
                                        // Take the screenshot from the upper left corner to the right bottom corner.
                                        gfxScreenShot.CopyFromScreen(
                                            Screen.PrimaryScreen.Bounds.X,
                                            Screen.PrimaryScreen.Bounds.Y,
                                            0, 0,
                                            Screen.PrimaryScreen.Bounds.Size,
                                            System.Drawing.CopyPixelOperation.SourceCopy);

                                        sourceScreenCapture.Packetize(bmpScreenShot);

                                        if (!Thread.Yield())
                                        {
                                            Thread.Sleep(9);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        server.Logger.LogException(ex);

                                        gfxScreenShot.Dispose();

                                        bmpScreenShot.Dispose();

                                        if (server != null && server.IsRunning) goto Start;
                                    }
                                }

                                int exit = 1;

                                if (exit == 1) return;
                            }
                        }
                    }));
                    
                    server.Start();

                    while (!server.IsRunning)
                    {
                        Thread.Sleep(0);
                    }

                    if (false.Equals(object.ReferenceEquals(captureThread, null)))
                    {
                        //captureThread.Priority = System.Threading.ThreadPriority.BelowNormal;

                        captureThread.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        #region Outils
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static bool IsValidURI(string uri)
        {
            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                return false;
            Uri tmp;
            if (!Uri.TryCreate(uri, UriKind.Absolute, out tmp))
                return false;
            return tmp.Scheme == Uri.UriSchemeHttp || tmp.Scheme == Uri.UriSchemeHttps;
        }

        internal static void DumpTrack(Media.Container.Track track)
        {
            Console.WriteLine();

            Console.WriteLine("Id: " + track.Id);

            Console.WriteLine("Name: " + track.Name);
            Console.WriteLine("Duration: " + track.Duration);

            Console.WriteLine("Type: " + track.MediaType);
            Console.WriteLine("Samples: " + track.SampleCount);

            if (track.MediaType == Media.Sdp.MediaType.audio)
            {
                Console.WriteLine("Codec: " + (track.CodecIndication.Length > 2 ? Encoding.UTF8.GetString(track.CodecIndication) : ((Media.Codecs.Audio.WaveFormatId)Media.Common.Binary.ReadU16(track.CodecIndication, 0, false)).ToString()));
                Console.WriteLine("Channels: " + track.Channels);
                Console.WriteLine("Sampling Rate: " + track.Rate);
                Console.WriteLine("Bits Per Sample: " + track.BitDepth);
            }
            else
            {
                Console.WriteLine("Codec: " + Encoding.UTF8.GetString(track.CodecIndication));
                Console.WriteLine("Frame Rate: " + track.Rate);
                Console.WriteLine("Width: " + track.Width);
                Console.WriteLine("Height: " + track.Height);
                Console.WriteLine("BitsPerPixel: " + track.BitDepth);
            }
            Console.WriteLine();
            Console.WriteLine();
        }

        private void TestContainerImplementations()
        {

            #region BaseMediaReader

            if (System.IO.Directory.Exists(@"C:\VTT\Video") || System.IO.Directory.Exists(@"C:\VTT\Video")) foreach (string fileName in System.IO.Directory.GetFiles(@"C:\VTT\Video").Concat(System.IO.Directory.GetFiles(@"C:\VTT\Video")))
                {
                    using (Media.Containers.BaseMedia.BaseMediaReader reader = new Media.Containers.BaseMedia.BaseMediaReader(fileName))
                    {
                        Console.WriteLine("-------------Path:" + reader.Source);
                        Console.WriteLine("-------------Total Size:" + reader.Length);
                        Console.WriteLine("-------------Root Box:" + reader.Root.ToString());
                        Console.WriteLine("-------------Boxes:");

                        int i = 0;
                        foreach (var box in reader)
                        {
                            Console.WriteLine(string.Format("-----Box[{0}]------------", i));
                            Console.WriteLine("-----Position:" + reader.Position);
                            Console.WriteLine("-----Offset: " + box.Offset);
                            Console.WriteLine("-----DataOffset: " + box.DataOffset);
                            Console.WriteLine("-----Complete: " + box.IsComplete);
                            Console.WriteLine("-----Name: " + box.ToString());
                            Console.WriteLine("-----LengthSize: " + box.LengthSize);
                            Console.WriteLine("-----DataSize: " + box.DataSize);
                            Console.WriteLine("-----TotalSize: " + box.TotalSize);
                            Console.WriteLine("-----IsUserDefinedNode: " + Media.Containers.BaseMedia.BaseMediaReader.IsUserDefinedNode(reader, box));
                            Console.WriteLine("-----UUID: " + Media.Containers.BaseMedia.BaseMediaReader.GetUniqueIdentifier(reader, box));
                            Console.WriteLine("-----ParentBox: " + Media.Containers.BaseMedia.BaseMediaReader.ParentBoxes.Contains(Media.Containers.BaseMedia.BaseMediaReader.ToUTF8FourCharacterCode(box.Identifier)));
                            Console.WriteLine("-----End box--------");
                            Console.WriteLine();
                            Console.WriteLine();
                            i++;
                        }
                        i = 0;

                        Console.WriteLine("-------------File Level Properties");
                        Console.WriteLine("-------------Created:" + reader.Created);
                        Console.WriteLine("-------------Last Modified:" + reader.Modified);
                        Console.WriteLine("-------------Movie Duration:" + reader.Duration);
                        Console.WriteLine("-------------Track Information------------");
                        foreach (var track in reader.GetTracks()) DumpTrack(track);

                        Console.WriteLine("--------End checking------------");
                    }
                }

            #endregion

            #region RiffReader
            /*
            if (System.IO.Directory.Exists(localPath + "/Media/Video/avi/")) foreach (string fileName in System.IO.Directory.GetFiles(localPath + "/Media/Video/avi/")) using (Media.Containers.Riff.RiffReader reader = new Media.Containers.Riff.RiffReader(fileName))
                    {
                        Console.WriteLine("Path:" + reader.Source);
                        Console.WriteLine("Total Size:" + reader.Length);

                        Console.WriteLine("Root Chunk:" + Media.Containers.Riff.RiffReader.ToFourCharacterCode(reader.Root.Identifier));

                        Console.WriteLine("File Level Information");

                        Console.WriteLine("Microseconds Per Frame:" + reader.MicrosecondsPerFrame);

                        Console.WriteLine("Max Bytes Per Seconds:" + reader.MaxBytesPerSecond);

                        Console.WriteLine("Flags:" + reader.Flags);
                        Console.WriteLine("HasIndex:" + reader.HasIndex);
                        Console.WriteLine("MustUseIndex:" + reader.MustUseIndex);
                        Console.WriteLine("IsInterleaved:" + reader.IsInterleaved);
                        Console.WriteLine("TrustChunkType:" + reader.TrustChunkType);
                        Console.WriteLine("WasCaptureFile:" + reader.WasCaptureFile);
                        Console.WriteLine("Copyrighted:" + reader.Copyrighted);

                        Console.WriteLine("Total Frames:" + reader.TotalFrames);

                        Console.WriteLine("Initial Frames:" + reader.InitialFrames);

                        Console.WriteLine("Streams:" + reader.Streams);

                        Console.WriteLine("Suggested Buffer Size:" + reader.SuggestedBufferSize);

                        Console.WriteLine("Width:" + reader.Width);

                        Console.WriteLine("Height:" + reader.Height);

                        Console.WriteLine("Reserved:" + reader.Reserved);

                        Console.WriteLine("Duration:" + reader.Duration);

                        Console.WriteLine("Created:" + reader.Created);

                        Console.WriteLine("Last Modified:" + reader.Modified);

                        Console.WriteLine("Chunks:");

                        foreach (var chunk in reader)
                        {
                            Console.WriteLine("Position:" + reader.Position);
                            Console.WriteLine("Offset: " + chunk.Offset);
                            Console.WriteLine("DataOffset: " + chunk.DataOffset);
                            Console.WriteLine("Complete: " + chunk.IsComplete);

                            string name = Media.Containers.Riff.RiffReader.ToFourCharacterCode(chunk.Identifier);

                            Console.WriteLine("Name: " + name);

                            //Show how the common type can be read.
                            if (Media.Containers.Riff.RiffReader.HasSubType(chunk)) Console.WriteLine("Type: " + Media.Containers.Riff.RiffReader.GetSubType(chunk));

                            Console.WriteLine("DataSize: " + chunk.DataSize);
                            Console.WriteLine("TotalSize: " + chunk.DataSize);
                        }

                        Console.WriteLine("Track Information:");

                        foreach (var track in reader.GetTracks()) DumpTrack(track);
                    }
            */
            #endregion

            #region MatroskaReader
            /*
            if (System.IO.Directory.Exists(localPath + "/Media/Video/mkv/")) foreach (string fileName in System.IO.Directory.GetFiles(localPath + "/Media/Video/mkv/"))
                {
                    using (Media.Containers.Matroska.MatroskaReader reader = new Media.Containers.Matroska.MatroskaReader(fileName))
                    {
                        Console.WriteLine("Path:" + reader.Source);
                        Console.WriteLine("Total Size:" + reader.Length);

                        Console.WriteLine("Root Element:" + reader.Root.ToString());

                        Console.WriteLine("File Level Information");

                        Console.WriteLine("EbmlVersion:" + reader.EbmlVersion);
                        Console.WriteLine("EbmlReadVersion:" + reader.EbmlReadVersion);
                        Console.WriteLine("DocType:" + reader.DocType);
                        Console.WriteLine("DocTypeVersion:" + reader.DocTypeVersion);
                        Console.WriteLine("DocTypeReadVersion:" + reader.DocTypeReadVersion);
                        Console.WriteLine("EbmlMaxIdLength:" + reader.EbmlMaxIdLength);
                        Console.WriteLine("EbmlMaxSizeLength:" + reader.EbmlMaxSizeLength);

                        Console.WriteLine("Elements:");

                        foreach (var element in reader)
                        {
                            Console.WriteLine("Name: " + element.ToString());
                            Console.WriteLine("Element Offset: " + element.Offset);
                            Console.WriteLine("Element Data Offset: " + element.DataOffset);
                            Console.WriteLine("Element DataSize: " + element.DataSize);
                            Console.WriteLine("Element TotalSize: " + element.TotalSize);
                            Console.WriteLine("Element.IsComplete: " + element.IsComplete);
                        }

                        Console.WriteLine("Movie Muxer Application:" + reader.MuxingApp);

                        Console.WriteLine("Movie Writing Applicatiopn:" + reader.WritingApp);

                        Console.WriteLine("Created:" + reader.Created);

                        Console.WriteLine("Modified:" + reader.Modified);

                        Console.WriteLine("Movie Duration:" + reader.Duration);

                        Console.WriteLine("Track Information:");

                        foreach (var track in reader.GetTracks()) DumpTrack(track);

                    }

                }
            */
            #endregion

            #region AsfReader
            /*
            if (System.IO.Directory.Exists(localPath + "/Media/Video/asf/")) foreach (string fileName in System.IO.Directory.GetFiles(localPath + "/Media/Video/asf/"))
                {
                    using (Media.Containers.Asf.AsfReader reader = new Media.Containers.Asf.AsfReader(fileName))
                    {
                        Console.WriteLine("Path:" + reader.Source);
                        Console.WriteLine("Total Size:" + reader.Length);

                        Console.WriteLine("Root Element:" + reader.Root.ToString());

                        Console.WriteLine("File Level Information");

                        Console.WriteLine("Created: " + reader.Created);
                        Console.WriteLine("Modified: " + reader.Modified);
                        Console.WriteLine("FileSize: " + reader.FileSize);
                        Console.WriteLine("NumberOfPackets: " + reader.NumberOfPackets);
                        Console.WriteLine("MinimumPacketSize: " + reader.MinimumPacketSize);
                        Console.WriteLine("MaximumPacketSize: " + reader.MaximumPacketSize);
                        Console.WriteLine("Duration: " + reader.Duration);
                        Console.WriteLine("PlayTime: " + reader.PlayTime);
                        Console.WriteLine("SendTime: " + reader.SendTime);
                        Console.WriteLine("PreRoll: " + reader.PreRoll);
                        Console.WriteLine("Flags: " + reader.Flags);
                        Console.WriteLine("IsBroadcast: " + reader.IsBroadcast);
                        Console.WriteLine("IsSeekable: " + reader.IsSeekable);

                        Console.WriteLine("Content Description");

                        Console.WriteLine("Title: " + reader.Title);
                        Console.WriteLine("Author: " + reader.Author);
                        Console.WriteLine("Copyright: " + reader.Copyright);
                        Console.WriteLine("Comment: " + reader.Comment);

                        Console.WriteLine("Objects:");

                        foreach (var asfObject in reader)
                        {
                            Console.WriteLine("Identifier:" + BitConverter.ToString(asfObject.Identifier));
                            Console.WriteLine("Name: " + asfObject.ToString());
                            Console.WriteLine("Position:" + reader.Position);
                            Console.WriteLine("Offset: " + asfObject.Offset);
                            Console.WriteLine("DataOffset: " + asfObject.DataOffset);
                            Console.WriteLine("Complete: " + asfObject.IsComplete);
                            Console.WriteLine("TotalSize: " + asfObject.TotalSize);
                            Console.WriteLine("DataSize: " + asfObject.DataSize);
                        }

                        Console.WriteLine("Track Information:");

                        foreach (var track in reader.GetTracks()) DumpTrack(track);
                    }

                }
            */
            #endregion

            #region MxfReader
            /*
            if (System.IO.Directory.Exists(localPath + "/Media/Video/mxf/")) foreach (string fileName in System.IO.Directory.GetFiles(localPath + "/Media/Video/mxf/"))
                {
                    using (Media.Containers.Mxf.MxfReader reader = new Media.Containers.Mxf.MxfReader(fileName))
                    {
                        Console.WriteLine("Path:" + reader.Source);
                        Console.WriteLine("Total Size:" + reader.Length);

                        Console.WriteLine("Root Object:" + reader.Root.ToString());

                        Console.WriteLine("Objects:");

                        foreach (var mxfObject in reader)
                        {
                            Console.WriteLine("Position:" + reader.Position);
                            Console.WriteLine("Offset: " + mxfObject.Offset);
                            Console.WriteLine("DataOffset: " + mxfObject.DataOffset);
                            Console.WriteLine("Complete: " + mxfObject.IsComplete);

                            string name = Media.Containers.Mxf.MxfReader.ToTextualConvention(mxfObject.Identifier);

                            Console.WriteLine("Identifier: " + BitConverter.ToString(mxfObject.Identifier));

                            Console.WriteLine("Category: " + Media.Containers.Mxf.MxfReader.GetCategory(mxfObject));

                            Console.WriteLine("Name: " + name);

                            Console.WriteLine("TotalSize: " + mxfObject.TotalSize);
                            Console.WriteLine("DataSize: " + mxfObject.DataSize);

                            //CompareUL?
                            if (name == "PartitionPack")
                            {
                                Console.WriteLine("Partition Type: " + Media.Containers.Mxf.MxfReader.GetPartitionKind(mxfObject));
                                Console.WriteLine("Partition Status: " + Media.Containers.Mxf.MxfReader.GetPartitionStatus(mxfObject));
                            }
                        }

                        Console.WriteLine("File Level Properties");

                        Console.WriteLine("Created: " + reader.Created);

                        Console.WriteLine("Modified: " + reader.Modified);

                        Console.WriteLine("HasRunIn:" + reader.HasRunIn);

                        Console.WriteLine("RunInSize:" + reader.RunInSize);

                        Console.WriteLine("HeaderVersion:" + reader.HeaderVersion);

                        Console.WriteLine("AlignmentGrid:" + reader.AlignmentGridByteSize);

                        Console.WriteLine("IndexByteCount:" + reader.IndexByteCount);

                        Console.WriteLine("OperationalPattern:" + reader.OperationalPattern);

                        Console.WriteLine("ItemComplexity:" + reader.ItemComplexity);

                        Console.WriteLine("PrefaceLastModifiedDate:" + reader.PrefaceLastModifiedDate);

                        Console.WriteLine("PrefaceVersion:" + reader.PrefaceVersion);

                        Console.WriteLine("Platform:" + reader.Platform);

                        Console.WriteLine("CompanyName:" + reader.CompanyName);

                        Console.WriteLine("ProductName:" + reader.ProductName);

                        Console.WriteLine("ProductVersion:" + reader.ProductVersion);

                        Console.WriteLine("ProductUID:" + reader.ProductUID);

                        Console.WriteLine("IdentificationModificationDate:" + reader.IdentificationModificationDate);

                        Console.WriteLine("MaterialCreationDate:" + reader.Created);

                        Console.WriteLine("MaterialModifiedDate:" + reader.Modified);

                        Console.WriteLine("Track Information:");

                        foreach (var track in reader.GetTracks()) DumpTrack(track);
                    }

                }
            */
            #endregion

            #region OggReader
            /*
            if (System.IO.Directory.Exists(localPath + "/Media/Video/ogg/")) foreach (string fileName in System.IO.Directory.GetFiles(localPath + "/Media/Video/ogg/"))
                {
                    using (Media.Containers.Ogg.OggReader reader = new Media.Containers.Ogg.OggReader(fileName))
                    {
                        Console.WriteLine("Path:" + reader.Source);
                        Console.WriteLine("Total Size:" + reader.Length);

                        Console.WriteLine("Root Page:" + reader.Root.ToString());

                        Console.WriteLine("Pages:");

                        foreach (var page in reader)
                        {
                            Console.WriteLine("Position:" + reader.Position);
                            Console.WriteLine("Offset: " + page.Offset);
                            Console.WriteLine("DataOffset: " + page.DataOffset);
                            Console.WriteLine("Complete: " + page.IsComplete);
                            Console.WriteLine("Name: " + page.ToString());
                            Console.WriteLine("HeaderFlags: " + Media.Containers.Ogg.OggReader.GetHeaderType(page));
                            Console.WriteLine("Size: " + page.TotalSize);
                        }


                        Console.WriteLine("File Level Properties");

                        Console.WriteLine("Created: " + reader.Created);

                        Console.WriteLine("Modified: " + reader.Modified);

                        Console.WriteLine("Track Information:");

                        foreach (var track in reader.GetTracks()) DumpTrack(track);
                    }

                }
            */
            #endregion

            #region NutReader
            /*
            if (System.IO.Directory.Exists(localPath + "/Media/Video/nut/")) foreach (string fileName in System.IO.Directory.GetFiles(localPath + "/Media/Video/nut/"))
                {
                    using (Media.Containers.Nut.NutReader reader = new Media.Containers.Nut.NutReader(fileName))
                    {
                        Console.WriteLine("Path:" + reader.Source);
                        Console.WriteLine("Total Size:" + reader.Length);

                        Console.WriteLine("Root Tag:" + Media.Containers.Nut.NutReader.ToTextualConvention(reader.Root.Identifier));

                        Console.WriteLine("Tags:");

                        foreach (var tag in reader)
                        {
                            Console.WriteLine("Position:" + reader.Position);
                            Console.WriteLine("Offset: " + tag.Offset);
                            Console.WriteLine("DataOffset: " + tag.DataOffset);
                            Console.WriteLine("Complete: " + tag.IsComplete);

                            if (Media.Containers.Nut.NutReader.IsFrame(tag))
                            {
                                Console.WriteLine("Frame:");
                                Console.WriteLine("FrameFlags: " + Media.Containers.Nut.NutReader.GetFrameFlags(reader, tag));
                                int streamId = Media.Containers.Nut.NutReader.GetStreamId(tag);
                                Console.WriteLine("StreamId: " + streamId);
                                Console.WriteLine("HeaderOptions: " + reader.HeaderOptions[streamId]);
                                Console.WriteLine("FrameHeader: " + BitConverter.ToString(Media.Containers.Nut.NutReader.GetFrameHeader(reader, tag)));
                            }
                            else
                                Console.WriteLine("Name: " + Media.Containers.Nut.NutReader.ToTextualConvention(tag.Identifier));

                            Console.WriteLine("TotalSize: " + tag.TotalSize);
                            Console.WriteLine("DataSize: " + tag.DataSize);
                        }

                        Console.WriteLine("File Level Properties");

                        Console.WriteLine("File Id String:" + reader.FileIdString);

                        Console.WriteLine("Created: " + reader.Created);

                        Console.WriteLine("Modified: " + reader.Modified);

                        Console.WriteLine("Version:" + reader.Version);

                        Console.WriteLine("IsStableVersion:" + reader.IsStableVersion);

                        if (reader.HasMainHeaderFlags) Console.WriteLine("HeaderFlags:" + reader.MainHeaderFlags);

                        Console.WriteLine("Stream Count:" + reader.StreamCount);

                        Console.WriteLine("MaximumDistance:" + reader.MaximumDistance);

                        Console.WriteLine("TimeBases:" + reader.TimeBases.Count());

                        Console.WriteLine("EllisionHeaderCount:" + reader.EllisionHeaderCount);

                        Console.WriteLine("HeaderOptions:" + reader.HeaderOptions.Count());

                        Console.WriteLine("Track Information:");

                        foreach (var track in reader.GetTracks()) DumpTrack(track);
                    }

                }
            */
            #endregion

            #region McfReader

            #endregion

            #region PacketizedElementaryStreamReader
            /*
            if (System.IO.Directory.Exists(localPath + "/Media/Video/pes/")) foreach (string fileName in System.IO.Directory.GetFiles(localPath + "/Media/Video/pes/"))
                {
                    using (Media.Containers.Mpeg.PacketizedElementaryStreamReader reader = new Media.Containers.Mpeg.PacketizedElementaryStreamReader(fileName))
                    {
                        Console.WriteLine("Path:" + reader.Source);
                        Console.WriteLine("Total Size:" + reader.Length);

                        Console.WriteLine("Root Element:" + reader.Root.ToString());

                        Console.WriteLine("Packets:");

                        foreach (var pesPacket in reader)
                        {
                            Console.WriteLine(pesPacket.ToString());
                            Console.WriteLine("Packets Offset: " + pesPacket.Offset);
                            Console.WriteLine("Packets Data Offset: " + pesPacket.DataOffset);
                            Console.WriteLine("Packets DataSize: " + pesPacket.DataSize);
                            Console.WriteLine("Packets TotalSize: " + pesPacket.TotalSize);
                            Console.WriteLine("Packet.IsComplete: " + pesPacket.IsComplete);
                        }

                        Console.WriteLine("Track Information:");

                        foreach (var track in reader.GetTracks()) DumpTrack(track);

                    }

                }
            */
            #endregion

            #region ProgramStreamReader
            /*
            if (System.IO.Directory.Exists(localPath + "/Media/Video/ps/")) foreach (string fileName in System.IO.Directory.GetFiles(localPath + "/Media/Video/ps/"))
                {
                    using (Media.Containers.Mpeg.ProgramStreamReader reader = new Media.Containers.Mpeg.ProgramStreamReader(fileName))
                    {
                        Console.WriteLine("Path:" + reader.Source);
                        Console.WriteLine("Total Size:" + reader.Length);

                        Console.WriteLine("Root Element:" + reader.Root.ToString());

                        Console.WriteLine("System Clock Rate:" + reader.SystemClockRate);

                        Console.WriteLine("Packets:");

                        foreach (var packet in reader)
                        {
                            Console.WriteLine(packet.ToString());
                            Console.WriteLine("Element Offset: " + packet.Offset);
                            Console.WriteLine("Element Data Offset: " + packet.DataOffset);
                            Console.WriteLine("Element DataSize: " + packet.DataSize);
                            Console.WriteLine("Element TotalSize: " + packet.TotalSize);
                            Console.WriteLine("Element.IsComplete: " + packet.IsComplete);
                        }

                        Console.WriteLine("Track Information:");

                        foreach (var track in reader.GetTracks()) DumpTrack(track);

                    }

                }
            */
            #endregion

            #region TransportStreamReader
            /*
            if (System.IO.Directory.Exists(localPath + "/Media/Video/ts/")) foreach (string fileName in System.IO.Directory.GetFiles(localPath + "/Media/Video/ts/"))
                {
                    using (Media.Containers.Mpeg.TransportStreamReader reader = new Media.Containers.Mpeg.TransportStreamReader(fileName))
                    {
                        Console.WriteLine("Path:" + reader.Source);
                        Console.WriteLine("Total Size:" + reader.Length);

                        Console.WriteLine("Root Element:" + reader.Root.ToString());

                        Console.WriteLine("Packets:");

                        foreach (var tsUnit in reader)
                        {
                            Console.WriteLine("Unit Type:" + tsUnit.ToString());
                            Console.WriteLine("Unit Offset: " + tsUnit.Offset);
                            Console.WriteLine("Unit Data Offset: " + tsUnit.DataOffset);
                            Console.WriteLine("Unit DataSize: " + tsUnit.DataSize);
                            Console.WriteLine("Unit TotalSize: " + tsUnit.TotalSize);
                            Console.WriteLine("Unit.IsComplete: " + tsUnit.IsComplete);
                            Console.WriteLine("PacketIdentifier: " + Media.Containers.Mpeg.TransportStreamReader.GetPacketIdentifier(reader, tsUnit.Identifier));
                            Console.WriteLine("Has Payload: " + Media.Containers.Mpeg.TransportStreamReader.HasPayload(reader, tsUnit));
                            Console.WriteLine("HasTransportPriority: " + Media.Containers.Mpeg.TransportStreamReader.HasTransportPriority(reader, tsUnit));
                            Console.WriteLine("HasTransportErrorIndicator: " + Media.Containers.Mpeg.TransportStreamReader.HasTransportErrorIndicator(reader, tsUnit));
                            Console.WriteLine("HasPayloadUnitStartIndicator: " + Media.Containers.Mpeg.TransportStreamReader.HasPayloadUnitStartIndicator(reader, tsUnit));
                            Console.WriteLine("ScramblingControl: " + Media.Containers.Mpeg.TransportStreamReader.GetScramblingControl(reader, tsUnit));
                            Console.WriteLine("ContinuityCounter: " + Media.Containers.Mpeg.TransportStreamReader.GetContinuityCounter(reader, tsUnit));
                            // See section 2.4.3.3 of 13818-1
                            Media.Containers.Mpeg.TransportStreamUnit.AdaptationFieldControl adaptationFieldControl = Media.Containers.Mpeg.TransportStreamReader.GetAdaptationFieldControl(reader, tsUnit);
                            Console.WriteLine("AdaptationFieldControl: " + adaptationFieldControl);
                            if (adaptationFieldControl >= Media.Containers.Mpeg.TransportStreamUnit.AdaptationFieldControl.AdaptationFieldOnly)
                            {
                                Console.WriteLine("AdaptationField Flags: " + Media.Containers.Mpeg.TransportStreamReader.GetAdaptationFieldFlags(reader, tsUnit));
                                Console.WriteLine("AdaptationField Data : " + BitConverter.ToString(Media.Containers.Mpeg.TransportStreamReader.GetAdaptationFieldData(reader, tsUnit)));
                            }

                        }

                        Console.WriteLine("Track Information:");

                        foreach (var track in reader.GetTracks()) DumpTrack(track);

                    }

                }
            */
            #endregion
        }
        #endregion


    }
}
