using Microsoft.ML;
using ProjetoIntregador.Dados.Model;
using ProjetoIntregador.ML.Modelo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoIntregador.ML.Consumo
{
    public class ConsomeModel
    {
        public ModelOutput Predict(ModelInput input, RegistroModelo registroModelo)
        {
            Stream stream = new MemoryStream(registroModelo.Modelo);
            // Create new MLContext
            MLContext mlContext = new MLContext();

            // Load model & create prediction engine            
            ITransformer mlModel = mlContext.Model.Load(stream, out var modelInputSchema);
            var predEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);

            // Use model to make prediction on input data
            ModelOutput result = predEngine.Predict(input);
            return result;
        }
    }
}
