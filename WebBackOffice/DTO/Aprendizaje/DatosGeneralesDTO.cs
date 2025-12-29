

namespace WebBackOffice.DTO.Aprendizaje
{
    public class DatosGeneralesDTO
    {
        public ICollection<CombosGeneral>? ListaDimensiones { get; set; }
        public ICollection<CombosGeneral>? ListaProductos { get; set; }
        public ICollection<CombosGeneral>? ListaColaboradores { get; set; }
    }
}
