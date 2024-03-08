public static class NumberExtensions
{
    public static string ToKMBTQ(this double num, int decimals = 2)
    {
        if (num == 0)
        {
            return "0";
        }

        // Define suffixes for magnitudes (only supports up to ~10^195)
        string[] suffixes = { "", "k", "M", "B", "T", "Qa", "Qu", "Sx", "Sp", "O", "N", "D", "UD", "DD", "TD", "QaD", "QiD", "SxD", 
            "SpD", "OD", "ND", "UD", "DD", "TD", "QaD", "QuD", "SxD", "SpD", "OD", "ND", "V", "UV", "DV", "TV", "QaV", "QuV", "SxV",
            "SpV", "OV", "NV", "Tg", "UTg", "DTg", "TTg", "QaTg", "QuTg", "SxTg", "SpTg", "OTg", "NTg", "Qd", "UQd", "DQd", "TQd", 
            "QaQd", "QuQd", "SxQd", "SpQd", "OQd", "NQd", "Qg", "UQg", "DQg", "TQg", "QaQg", "QuQg", "SxQg", "SpQg", "OQg", "NQg", 
            "Sg", "USg", "DSg", "TSg", "QaSg"};
        int suffixIndex = 0;

        // Determine the suffix index based on the number's magnitude
        while (num >= 1000 && suffixIndex < suffixes.Length - 1)
        {
            num /= 1000;
            suffixIndex++;
        }

        // Handle very large numbers beyond defined suffixes
        if (suffixIndex == suffixes.Length - 1 && num >= 1000)
        {
            // Just use the last suffix with scientific notation for the number
            return num.ToString("0." + new string('0', decimals) + "e+###") + suffixes[suffixIndex];
        }

        // Format the number with the appropriate suffix
        return Math.Round(num, decimals).ToString("N" + decimals) + suffixes[suffixIndex];
    }
}
