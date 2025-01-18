using System.Text.RegularExpressions;

namespace Shop_Classix.Helper
{
    public static class Slug
    {
        public static string GenerateSlug(string phrase)
        {
            // Chuyển chuỗi về chữ thường và thay thế khoảng trắng bằng dấu gạch nối
            string str = phrase.ToLower();

            // Thay thế các ký tự không phải chữ cái hoặc chữ số bằng dấu gạch nối
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim(); // Thay thế nhiều khoảng trắng liên tiếp bằng 1
            str = Regex.Replace(str, @"\s", "-"); // Thay thế khoảng trắng bằng dấu gạch nối

            return str;
        }
    }
}
