namespace CheckupMedico.Transversal.Exception
{
    using System;
    public class NotFoundException : Exception
    {
        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}
