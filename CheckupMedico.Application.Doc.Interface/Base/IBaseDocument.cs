namespace CheckupMedico.Application.Doc.Interface.Base
{
    public interface IBaseDocument<TEntrada> where TEntrada : class
    {
        void Build(TEntrada data);
        byte[] GetDoc();
    }
}
