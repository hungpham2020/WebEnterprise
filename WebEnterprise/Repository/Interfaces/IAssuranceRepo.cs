﻿using WebEnterprise.Models;
using WebEnterprise.Models.DTO;

namespace WebEnterprise.Repository.Interfaces
{
    public interface IAssuranceRepo
    {
        public IQueryable<UserDTO> GetAllAssurances();

        public Task<CustomUser> AddAssurance(string userName, string fullName, string email, List<Department> departs);

        public UserDTO GetEditAssurance(string id);

        public CustomUser GetAssuranceById(string id);

        public CustomUser EditAssurance(UserDTO res);

        public bool DeleteAssurance(string id);
    }
}
