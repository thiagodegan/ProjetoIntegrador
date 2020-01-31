using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjetoIntregador.Dados.Model;

namespace ProjetoIntregador.Dados.Bll.Contract
{
    public interface ITreinarModelos
    {
        List<RegistroFilial> ListarFiliais();
        List<RegistroCategoria> ListarCategorias();
        Task Treinar();
    }
}