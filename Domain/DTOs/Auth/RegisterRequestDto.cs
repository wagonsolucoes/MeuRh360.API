﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Auth
{
    public class RegisterRequestDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Role { get; set; }
    }
}
