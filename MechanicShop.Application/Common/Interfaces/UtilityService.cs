namespace MechanicShop.Application.Common.Interfaces
{
    public static class UtilityService
    {
        public static string MaskEmail(string email)
        {
            //google@gmail.com
            int atIndex = email.IndexOf('@');  //a@gmail.com
            if (atIndex <= 1)
            {
                return $"****{email.AsSpan(atIndex)}";  //****@gmail.com
            }
            return email[0] + "****" + email[atIndex - 1] + email[atIndex..]; //g****e@gmail.com

        }
    }
}
