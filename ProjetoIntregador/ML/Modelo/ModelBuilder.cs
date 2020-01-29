using Microsoft.ML;
using Microsoft.ML.Data;
using ProjetoIntregador.Dados.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoIntregador.ML.Modelo
{
    public class ModelBuilder
    {
        public MLContext mlContext = new MLContext();
        public RegistroModelo CreateModel(List<ModelInput> modelInputs)
        {
            IDataView trainingDataView = mlContext.Data.LoadFromEnumerable(modelInputs);

            IEstimator<ITransformer> trainingPipeline = BuildTrainingPipeline(mlContext);

            var crossValidationResults = Evalute(mlContext, trainingDataView, trainingPipeline);

            ITransformer mlModel = TrainModel(mlContext, trainingDataView, trainingPipeline);

            MemoryStream memoryStream = SaveModel(mlContext, mlModel, trainingDataView.Schema);

            RegistroModelo registroModelo = new RegistroModelo
            {
                LossFunc = crossValidationResults != null ? crossValidationResults.Select(r => r.Metrics.LossFunction).Average() : 0,
                MeanAbsoluteError = crossValidationResults != null ? crossValidationResults.Select(r => r.Metrics.MeanAbsoluteError).Average() : 0,
                MeanSquaredError = crossValidationResults != null ? crossValidationResults.Select(r => r.Metrics.MeanSquaredError).Average() : 0,
                RootMeanSquaredError = crossValidationResults != null ? crossValidationResults.Select(r => r.Metrics.RootMeanSquaredError).Average() : 0,
                RSquared = crossValidationResults != null ? crossValidationResults.Select(r => r.Metrics.RSquared).Average() : 0,
                Modelo = memoryStream.ToArray()
            };

            return registroModelo;
        }

        private MemoryStream SaveModel(MLContext mlContext, ITransformer mlModel, DataViewSchema schema)
        {
            MemoryStream memoryStream = new MemoryStream();
            mlContext.Model.Save(mlModel, schema, memoryStream);
            return memoryStream;
        }

        private ITransformer TrainModel(MLContext mlContext, IDataView trainingDataView, IEstimator<ITransformer> trainingPipeline)
        {
            ITransformer model = trainingPipeline.Fit(trainingDataView);
            return model;
        }

        private IEnumerable<TrainCatalogBase.CrossValidationResult<RegressionMetrics>> Evalute(MLContext mlContext, IDataView trainingDataView, IEstimator<ITransformer> trainingPipeline)
        {
            try
            {
                var crossValidationResults = mlContext.Regression.CrossValidate(trainingDataView, trainingPipeline, numberOfFolds: 5, labelColumnName: "Label");
                return crossValidationResults;
            }
            catch
            {
                return null;
            }
        }

        private IEstimator<ITransformer> BuildTrainingPipeline(MLContext mlContext)
        {
            var dataProcessPipeline = mlContext.Transforms.Concatenate("Features", new[] { "ANO", "MES_FEV", "MES_MAR", "MES_ABR", "MES_MAI", "MES_JUN", "MES_JUL", "MES_AGO", "MES_SET", "MES_OUT", "MES_NOV", "MES_DEZ", "SEM_MES_SEG", "SEM_MES_TER", "SEM_MES_QUA", "SEM_MES_QUI", "DIA_SEG", "DIA_TER", "DIA_QUA", "DIA_QUI", "DIA_SEX", "DIA_SAB" });
            var trainer = mlContext.Regression.Trainers.LightGbm(labelColumnName: "Label", featureColumnName: "Features", minimumExampleCountPerLeaf: 2, learningRate: 0.001, numberOfIterations: 200);
            var trainingPipeline = dataProcessPipeline.Append(trainer);
            return trainingPipeline;
        }
    }
}
