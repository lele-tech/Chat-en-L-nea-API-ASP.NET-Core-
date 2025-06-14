using System.ComponentModel.DataAnnotations;

namespace AppWebBibliotecaConsumo.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }


        public string GroupName { get; set; }


        public string UserName { get; set; }


        public string Mensaje{ get; set; }


        public DateTime TimeSent { get; set; } = DateTime.Now;

    }
}
