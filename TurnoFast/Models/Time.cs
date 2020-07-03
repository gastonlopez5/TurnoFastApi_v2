namespace TurnoFastApi.Models
{
    public class Time
    {
        public Time(int hora, int minuto, int nano, int segundo)
        {
            this.hour = hora;
            this.minute = minuto;
            this.nano = nano;
            this.second = segundo;
        }
        public int hour { get; set; }
        public int minute { get; set; }
        public int nano { get; set; }
        public int second { get; set; }
    }
}