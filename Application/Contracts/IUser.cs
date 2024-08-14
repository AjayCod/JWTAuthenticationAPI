using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IUser
    {
        Task<RegistrationResponce> RegistrationUserAsync(RigesterUserDTO rigesterUserDTO);
        Task<LoginResponse> LoginUserAsync(LoginDTO loginDTO);
    }
}
