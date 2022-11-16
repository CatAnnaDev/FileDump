using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace FileDump
{
    internal class Program
    {
        static string path = @"\test.meow";
        static void Main(string[] args)
        {
            Console.WriteLine(getMimeFromFile(path));

            Console.WriteLine(MimeTypeFrom(path));

            Console.WriteLine(GetMimeFromRegistry(path));
        }

        private static string GetMimeFromRegistry(string Filename)
        {
            string mime = "application/octetstream";
            string ext = Path.GetExtension(Filename).ToLower();
            RegistryKey? rk = Registry.ClassesRoot.OpenSubKey(ext);
            if (rk != null && rk.GetValue("Content Type") != null)
                mime = rk.GetValue("Content Type").ToString();
            return mime;
        }

        public static string getMimeFromFile(string file)
        {
            IntPtr mimeout;
            if (!File.Exists(file))
                throw new FileNotFoundException(file + " not found");

            int MaxContent = (int)new FileInfo(file).Length;
            if (MaxContent > 4096) MaxContent = 4096;
            FileStream fs = File.OpenRead(file);


            byte[] buf = new byte[MaxContent];
            fs.Read(buf, 0, MaxContent);
            fs.Close();
            int result = import.FindMimeFromData(IntPtr.Zero, file, buf, MaxContent, null, 0, out mimeout, 0);

            if (result != 0)
                throw Marshal.GetExceptionForHR(result);
            string mime = Marshal.PtrToStringUni(mimeout);
            Marshal.FreeCoTaskMem(mimeout);
            return mime;
        }

        public static string MimeTypeFrom(string mimeProposed)
        {
            byte[] dataBytes = new byte[256];

            using (FileStream fs = new FileStream(mimeProposed, FileMode.Open))
            {
                if (fs.Length >= 256)
                    fs.Read(dataBytes, 0, 256);
                else
                    fs.Read(dataBytes, 0, (int)fs.Length);
            }

            if (dataBytes == null)
                throw new ArgumentNullException("dataBytes");
            string mimeRet = String.Empty;
            IntPtr suggestPtr = IntPtr.Zero, filePtr = IntPtr.Zero, outPtr = IntPtr.Zero;
            if (mimeProposed != null && mimeProposed.Length > 0)
            {
                suggestPtr = Marshal.StringToCoTaskMemUni(mimeProposed);
                global::System.Console.WriteLine(suggestPtr);
                mimeRet = mimeProposed;
            }
            int ret = import.FindMimeFromData(IntPtr.Zero, null, dataBytes, dataBytes.Length, mimeProposed, 0, out outPtr, 0);
            if (ret == 0 && outPtr != IntPtr.Zero)
            {
                mimeRet = Marshal.PtrToStringUni(outPtr);
                Marshal.FreeCoTaskMem(outPtr);
                return mimeRet;

            }
            return mimeRet;
        }
    }
}