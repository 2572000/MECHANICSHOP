using MechanicShop.Domain.Employees;
using MechanicShop.Domain.Identity;
using MechanicShop.Tests.Common.Employees;
using Xunit;

namespace MechanicShop.Domain.UnitTest.Employees
{
    public class EmployeeTests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var id = Guid.NewGuid();
            const string firstName = "John";
            const string lastName = "Doe";
            const Role role = Role.Labor;

            var result=EmployeeFactory.CreateEmployee(id, firstName, lastName,role);

            Assert.True(result.IsSuccess);

            var employee = result.Value;

            Assert.Equal(id, employee.Id);
            Assert.Equal(firstName, employee.FirstName);
            Assert.Equal(lastName, employee.LastName);
            Assert.Equal(role, employee.Role);
            Assert.Equal("John Doe", employee.FullName);

        }

        [Fact]
        public void Create_WithEmptyId_ShouldFail()
        {
            var result = EmployeeFactory.CreateEmployee(Guid.Empty, "John", "Doe", Role.Manager);

            Assert.True(result.IsError);
            Assert.Equal(EmployeeErrors.IdRequired.Code, result.TopError.Code);
            Assert.Equal(EmployeeErrors.IdRequired.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithEmptyFirstName_ShouldFail()
        {
            var result = EmployeeFactory.CreateEmployee(Guid.NewGuid(), " ", "Doe", Role.Manager);

            Assert.True(result.IsError);
            Assert.Equal(EmployeeErrors.FirstNameRequired.Code, result.TopError.Code);
            Assert.Equal(EmployeeErrors.FirstNameRequired.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithEmptyLastName_ShouldFail()
        {
            var result = EmployeeFactory.CreateEmployee(Guid.NewGuid(), "John", " ", Role.Manager);

            Assert.True(result.IsError);
            Assert.Equal(EmployeeErrors.LastNameRequired.Code, result.TopError.Code);
            Assert.Equal(EmployeeErrors.FirstNameRequired.Description, result.TopError.Description);
        }

        [Fact]
        public void Create_WithInvalidRole_ShouldFail()
        {
            var result = Employee.Create(Guid.NewGuid(), "John", "Doe", (Role)999);

            Assert.True(result.IsError);
            Assert.Equal(EmployeeErrors.RoleInvalid.Code, result.TopError.Code);
            Assert.Equal(EmployeeErrors.RoleInvalid.Description, result.TopError.Description);
        }
    }
}
