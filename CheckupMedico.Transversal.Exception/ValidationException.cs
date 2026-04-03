namespace CheckupMedico.Transversal.Exception
{
    using System;
    public class ValidationException : Exception
    {
        public List<string> Errors { get; }

        public ValidationException(List<string> errors)
            : base("Error de validación")
        {
            Errors = errors;
        }
    }
}
