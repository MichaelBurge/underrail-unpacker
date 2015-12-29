using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.IO.Compression;

namespace ConsoleApplication2
{
    class Program
    {
        static void Unpack(Stream input, Stream output)
        {
            Guid guid = new Guid("{838B53F9-361F-4332-BAAE-0D17865D0854}");
            byte[] guidBytes = guid.ToByteArray();

            int guidSize = guidBytes.Length; // 16
            int intSize = 8;
            byte[] actualGuid = new byte[guidSize];
            byte[] version = new byte[intSize];

            input.Read(actualGuid, 0, guidSize);
            input.Read(version, 0, intSize);
            
            GZipStream gzipStream = new GZipStream(input, CompressionMode.Decompress);

            gzipStream.CopyTo(output);
            output.Flush();
        }
        static void Pack(Stream input, Stream output)
        {
            // Obtained from inspecting a global.dat file
            byte[] versionBytes = {
                                      // Guid
                                       0xF9, 0x53, 0x8B, 0x83, 0x1F, 0x36, 0x32, 0x43, 0xBA, 0xAE, 0x0D, 0x17, 0x86, 0x5D, 0x08, 0x54,
                                       // Version
                                       0x41, 0xF1, 0x4C, 0x31, 0x41, 0x00, 0x00, 0x00
                                  };
            output.Write(versionBytes, 0, versionBytes.Length);
            GZipStream gzipStream = new GZipStream(output, CompressionMode.Compress);
            input.CopyTo(gzipStream);
            gzipStream.Flush();
            gzipStream.Close();
        }
        static void Main(string[] args)
        {
            string compressOption = args[0];
            Stream stdin = System.Console.OpenStandardInput();
            Stream stdout = System.Console.OpenStandardOutput();
            if (compressOption == "pack") {
                Pack(stdin, stdout);
            } else if (compressOption == "unpack") {
                Unpack(stdin, stdout);
            } else {
                throw new Exception("Unknown compression option");
            }
        }
    }
}
