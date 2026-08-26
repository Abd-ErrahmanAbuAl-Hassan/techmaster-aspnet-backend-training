namespace Task_01_REST_Routing_Drill_Pack.Services
{
    public class ConverterService
    {
        public decimal ConvertCelsiusToFahrenheit(decimal celsius)
        {
            return celsius * 9 / 5 + 32;
        }
    }
}
