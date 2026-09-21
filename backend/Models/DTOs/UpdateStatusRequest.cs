using System.ComponentModel.DataAnnotations;
using backend.Models.Entities;

namespace backend.Models.DTOs;

/// <summary>Body for approving or rejecting an application.</summary>
public class UpdateStatusRequest
{
    [Required]
    public ApplicationStatus Status { get; set; }
}
