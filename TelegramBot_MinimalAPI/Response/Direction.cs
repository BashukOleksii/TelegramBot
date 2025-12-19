namespace TelegramBot_MinimalAPI.Response
{
    public static class Direction
    {
        public static string GetDirectionFromNumber(int? number)
        {
            if (number is null)
                return "Пн";

            var diretions = new[] 
            { 
                "Пн",
                "Пн-Сх",
                "Сх",
                "Пд-Сх",
                "Пд",
                "Пд-Зх",
                "Зх",
                "Пн-Зх",
            };

            int index = (int)(number + 22.5) / 45;
            
            return diretions[index%8];
        }
    }
}
