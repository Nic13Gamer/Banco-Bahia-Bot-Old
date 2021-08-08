namespace BancoBahiaBot.Utils
{
    class StringUtils
    {
        public static string RemoveAccents(string text)
        {
            var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }

        public static string GetAllRemainderTextAfter(string[] array, int index, bool hasSpace = true)
        {
            string output = string.Empty;

            for (int i = 0; i < array.Length; i++)
            {
                if (i <= index) continue;

                output += array[i];
                if (hasSpace)
                    output += " ";
            }

            return output.Trim();
        }
    }
}
