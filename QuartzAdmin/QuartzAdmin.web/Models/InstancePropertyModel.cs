using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuartzAdmin.web.Models;

public class InstancePropertyModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int InstancePropertyID { get; set; }

    public InstanceModel ParentInstance { get; set; } = null!;

    [Required]
    public string PropertyName { get; set; } = string.Empty;

    [Required]
    public string PropertyValue { get; set; } = string.Empty;
}
