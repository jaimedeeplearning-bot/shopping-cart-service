using System.Threading.Tasks;

namespace ShoppingCartService.Interfaces
{
    /// <summary>
    /// Interfaz para calcular impuestos
    /// </summary>
    public interface ITaxService
    {
        /// <summary>
        /// Obtiene la tasa de impuesto actual
        /// </summary>
        /// <returns>La tasa de impuesto como porcentaje (ej: 8.0 para 8%)</returns>
        Task<decimal> GetTaxRateAsync();

        /// <summary>
        /// Calcula el monto del impuesto
        /// </summary>
        /// <param name="baseAmount">Monto base sin impuestos</param>
        /// <returns>Monto del impuesto calculado</returns>
        decimal CalculateTax(decimal baseAmount);
    }
}
