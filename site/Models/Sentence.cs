using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace site.Models
{
    public class Sentence
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string SentenceText { get; set; }
        [Required]
        public int NumberOfAnswers { get; set; }
        public int NumberOfWarnings { get; set; }
    }
}