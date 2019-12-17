using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;


namespace Huihuinga.Models
{
    public class ConcreteConferenceCreateViewModel 
    {
        public EventCenter[] EventCenters { get; set; }
        public Guid abstractConferenceId { get; set; }
        [Required(ErrorMessage = "Debes agregar un centro de eventos")]
        public Guid centerId { get; set; }
        [Required]
        [Remote(action: "VerifyNewConcreteConference", controller: "ConcreteConference", 
                 ErrorMessage = "Esta instancia ya existe", AdditionalFields = "abstractConferenceId")]
        public string name { get; set; }
        [Required]
        public DateTime starttime { get; set; }
        [Required]
        public DateTime endtime { get; set; }
        [Required]
        public int Maxassistants { get; set; }
        public IFormFile Photo { get; set; }
    }
}
