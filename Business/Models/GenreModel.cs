#nullable disable

using DataAccess.Entities;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Business.Models
{
    public class GenreModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        // You may add additional properties if needed

        [DisplayName("Movies")]
        public List<Movie> Movies { get; set; }
    }


}