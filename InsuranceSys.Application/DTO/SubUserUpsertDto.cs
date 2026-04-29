namespace InsuranceSys.Application.DTO
{
    public sealed class SubUserUpsertDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Role { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

