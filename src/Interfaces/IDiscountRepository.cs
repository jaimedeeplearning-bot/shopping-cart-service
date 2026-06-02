using ShoppingCartService.Models;
using System.Threading.Tasks;

namespace ShoppingCartService.Interfaces
{
    /// <summary>
    /// Interfaz para acceder a los códigos de descuento desde la base de datos
    /// </summary>
    public interface IDiscountRepository
    {
        /// <summary>
        /// Obtiene un código de descuento por su valor
        /// </summary>
        /// <param name="code">El código de descuento</param>
        /// <returns>El objeto DiscountCode si existe, null si no</returns>
        Task<DiscountCode> GetDiscountCodeAsync(string code);
    }
}
