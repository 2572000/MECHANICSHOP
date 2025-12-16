using MechanicShop.Domain.Common.Results;

namespace MechanicShop.Domain.Employees
{
    public static class EmployeeErrors
    {
        public static Error IdRequired =>
            Error.Validation("Employee.Id.Required", "Employee Id is required.");
        public static Error FirstNameRequired =>
            Error.Validation("Employee.FirstName.Required", "Employee first name is required.");
        public static Error LastNameRequired =>
            Error.Validation("Employee.LastName.Required", "Employee last name is required.");
        public static Error RoleInvalid =>
            Error.Validation("Employee.Role.Invalid", "Invalid role assigned to employee.");
    }
}
