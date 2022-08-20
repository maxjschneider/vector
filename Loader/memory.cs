using System.IO;
using System.IO.MemoryMappedFiles;

namespace Loader
{
    class memory
    {
        MemoryMappedFile file;

        public memory(string name, int size = 80000000)
        {
            try
            {
                file = MemoryMappedFile.CreateNew(@"Global\" + name, size);
            } catch {
                System.Windows.MessageBox.Show(
                    "Failed to create memory mapped file.\n\nMake sure any other instances of Vector are closed,\nand that this process has adminstrator privileges.\n\nIf this error persists, restart your PC.", 
                    "Error", 
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Error);

                System.Environment.Exit(1);
            }
        }

        public void write(byte[] data)
        {
            try
            {
                using (MemoryMappedViewStream stream = file.CreateViewStream())
                {
                    BinaryWriter writer = new BinaryWriter(stream);

                    writer.Write(data);
                    writer.Flush();
                }

                return;
            }
            catch
            {
                System.Windows.MessageBox.Show(
                    "Failed to create write to memory mapped file.\n\nMake sure any other instances of Vector are closed,\nand that this process has adminstrator privileges.\n\nIf this error persists, restart your PC.",
                    "Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);

                System.Environment.Exit(1);
            }
        }
    }
}
