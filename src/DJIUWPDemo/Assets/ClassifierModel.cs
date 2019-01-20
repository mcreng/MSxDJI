using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Storage;
using Windows.AI.MachineLearning.Preview;

// f58aba18-f399-45bd-a4bc-0b38cca6bebf_41b133f2-2a15-418a-a526-4086607dcba8

namespace DJIDemo
{
    public sealed class F58aba18_x002D_f399_x002D_45bd_x002D_a4bc_x002D_0b38cca6bebf_41b133f2_x002D_2a15_x002D_418a_x002D_a526_x002D_4086607dcba8ModelInput
    {
        public VideoFrame data { get; set; }
    }

    public sealed class F58aba18_x002D_f399_x002D_45bd_x002D_a4bc_x002D_0b38cca6bebf_41b133f2_x002D_2a15_x002D_418a_x002D_a526_x002D_4086607dcba8ModelOutput
    {
        public IList<string> classLabel { get; set; }
        public IDictionary<string, float> loss { get; set; }
        public F58aba18_x002D_f399_x002D_45bd_x002D_a4bc_x002D_0b38cca6bebf_41b133f2_x002D_2a15_x002D_418a_x002D_a526_x002D_4086607dcba8ModelOutput()
        {
            this.classLabel = new List<string>();
            this.loss = new Dictionary<string, float>()
            {
                { "Cleaner", float.NaN },
                { "Crane", float.NaN },
                { "Excavator", float.NaN },
                { "Mixer", float.NaN },
            };
        }
    }

    public sealed class F58aba18_x002D_f399_x002D_45bd_x002D_a4bc_x002D_0b38cca6bebf_41b133f2_x002D_2a15_x002D_418a_x002D_a526_x002D_4086607dcba8Model
    {
        private LearningModelPreview learningModel;
        public static async Task<F58aba18_x002D_f399_x002D_45bd_x002D_a4bc_x002D_0b38cca6bebf_41b133f2_x002D_2a15_x002D_418a_x002D_a526_x002D_4086607dcba8Model> CreateF58aba18_x002D_f399_x002D_45bd_x002D_a4bc_x002D_0b38cca6bebf_41b133f2_x002D_2a15_x002D_418a_x002D_a526_x002D_4086607dcba8Model(StorageFile file)
        {
            LearningModelPreview learningModel = await LearningModelPreview.LoadModelFromStorageFileAsync(file);
            F58aba18_x002D_f399_x002D_45bd_x002D_a4bc_x002D_0b38cca6bebf_41b133f2_x002D_2a15_x002D_418a_x002D_a526_x002D_4086607dcba8Model model = new F58aba18_x002D_f399_x002D_45bd_x002D_a4bc_x002D_0b38cca6bebf_41b133f2_x002D_2a15_x002D_418a_x002D_a526_x002D_4086607dcba8Model();
            model.learningModel = learningModel;
            return model;
        }
        public async Task<F58aba18_x002D_f399_x002D_45bd_x002D_a4bc_x002D_0b38cca6bebf_41b133f2_x002D_2a15_x002D_418a_x002D_a526_x002D_4086607dcba8ModelOutput> EvaluateAsync(F58aba18_x002D_f399_x002D_45bd_x002D_a4bc_x002D_0b38cca6bebf_41b133f2_x002D_2a15_x002D_418a_x002D_a526_x002D_4086607dcba8ModelInput input) {
            F58aba18_x002D_f399_x002D_45bd_x002D_a4bc_x002D_0b38cca6bebf_41b133f2_x002D_2a15_x002D_418a_x002D_a526_x002D_4086607dcba8ModelOutput output = new F58aba18_x002D_f399_x002D_45bd_x002D_a4bc_x002D_0b38cca6bebf_41b133f2_x002D_2a15_x002D_418a_x002D_a526_x002D_4086607dcba8ModelOutput();
            LearningModelBindingPreview binding = new LearningModelBindingPreview(learningModel);
            binding.Bind("data", input.data);
            binding.Bind("classLabel", output.classLabel);
            binding.Bind("loss", output.loss);
            LearningModelEvaluationResultPreview evalResult = await learningModel.EvaluateAsync(binding, string.Empty);
            return output;
        }
    }
}
