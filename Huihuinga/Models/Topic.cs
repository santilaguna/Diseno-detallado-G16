using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Huihuinga.Models
{
    public class Topic
    {
        public Guid id { get; set; }
        [Required(ErrorMessage = "Debes agregar un nombre")]
        [Remote(action: "VerifyNewTopic", controller: "Topic", ErrorMessage = "Este tema ya existe")]
        public string name { get; set; }
        [Required(ErrorMessage = "Debes agregar una descripción")]
        public string description { get; set; }
        public List<EventTopic> EventTopics { get; set; }
    }
}
