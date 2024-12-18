using System.Text;

namespace HexStringToStringConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("请将仅包含十六进制或十进制 (每个 Byte 之间用空格隔开, 文件名包含 ByteString) 字符串的 .log 文件拖放到程序上执行.");
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                if (i == 0)
                {
                    Console.WriteLine($"----------------------------------------------------------------------------------------------------{Environment.NewLine}");
                }

                string filePath = args[i];
                Console.WriteLine($"准备转换! 文件名称: {filePath}{Environment.NewLine}");

                // 检查文件是否存在
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("指定的文件不存在.");
                    return;
                }

                bool isByteString = false;
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                if (fileName.Contains("ByteString", StringComparison.OrdinalIgnoreCase))
                {
                    isByteString = true;
                }

                // 读取文件内容
                string hexString = File.ReadAllText(filePath);

                // 确保文件内容是十六进制格式的字符串, 去除空格和换行符等
                //hexString = hexString.Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");

                //hexString = "48 65 6C 6C 6F 20 57 6F 72 6C 64 21"; // "Hello World!" 的十六进制表示
                if (isByteString)
                {
                    Console.WriteLine($"输入十进制字符串: {Environment.NewLine}{hexString}{Environment.NewLine}");
                    hexString = ByteStringToHexString(hexString);
                }
                Console.WriteLine($"输入十六进制字符串: {Environment.NewLine}{hexString}{Environment.NewLine}");
                string result = HexStringToString(hexString);
                Console.WriteLine($"输出字符串: {Environment.NewLine}{result}{Environment.NewLine}");

                // 构造新文件名, 添加 _ParsedUnformattedRecipeBody 后缀和日期时间
                string dateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss"); // 格式化日期和时间为 yyyyMMdd_HHmmss
                string newFileName = $"{Path.GetFileNameWithoutExtension(filePath)}_ParsedUnformattedRecipeBody_{dateTime}.log";
                string newFilePath = Path.Combine(Path.GetDirectoryName(filePath), newFileName);

                // 将结果写入新的文件
                File.WriteAllText(newFilePath, result);

                Console.WriteLine($"转换成功! 文件已保存为: {newFilePath}{Environment.NewLine}");
                Console.WriteLine($"----------------------------------------------------------------------------------------------------{Environment.NewLine}");
            }

            Console.WriteLine("按回车键关闭窗口...");
            Console.Read();
        }

        static string HexStringToString(string hexString)
        {
            if (string.IsNullOrEmpty(hexString))
                throw new ArgumentNullException("hexString cannot be null of empty");

            hexString = new string(hexString.Where(c => !char.IsWhiteSpace(c)).ToArray());

            if (hexString.Length % 2 != 0)
                throw new ArgumentException("hexString length must be even.");

            try
            {
                byte[] bytes = new byte[hexString.Length / 2];
                for (int i = 0; i < hexString.Length; i += 2)
                {
                    string hexByte = hexString.Substring(i, 2);
                    bytes[i / 2] = Convert.ToByte(hexByte, 16);
                }
                return Encoding.UTF8.GetString(bytes);
            }
            catch (FormatException)
            {
                throw new ArgumentException("hexString contains invalid characters.");
            }
            catch (OverflowException)
            {
                throw new ArgumentException("hexString contains invalid characters.");
            }
        }

        // 将类似以下格式的十进制 Byte 字符串 "60 80 82 79" 转换成类似以下格式的十六进制字符串 "3C 50 52 4F"
        static string ByteStringToHexString(string byteString)
        {
            if (string.IsNullOrEmpty(byteString))
                throw new ArgumentNullException("hexString cannot be null of empty");

            string[] byteArrayString = byteString.Split(' ').Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
            byte[] byteArray = byteArrayString.Select(s => byte.Parse(s)).ToArray();

            StringBuilder hexBuilder = new ();
            for (int i = 0; i < byteArray.Length; i++) {
                if (i > 0) hexBuilder.Append(' ');
                hexBuilder.Append(byteArray[i].ToString("X2"));
            }

            return hexBuilder.ToString();
        }
    }
}
