using System.Collections.Generic;

namespace Physics {

    public enum CommonForceGeneratorType {
        Gravity
    }

    public class ForceGeneratorFactory {

        public readonly static ForceGeneratorFactory Instance = new ForceGeneratorFactory();

        private Dictionary<CommonForceGeneratorType, IForceGenerator> _commonGenerators;

        public void InitFactory(float gravity) {
            InitCommonForceGenerators(gravity);
        }

        private void InitCommonForceGenerators(float gravity) {
            _commonGenerators = new Dictionary<CommonForceGeneratorType, IForceGenerator>();
            _commonGenerators.Add(CommonForceGeneratorType.Gravity, new GravityGenerator(gravity));
        }

        public IForceGenerator GetForceGenerator(CommonForceGeneratorType type) {
            if (_commonGenerators.TryGetValue(type, out IForceGenerator fg)) {
                return fg;
            }
            return null;
        }
    }

}