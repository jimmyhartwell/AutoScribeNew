using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP.Helpers {
    public class Enums {
        public enum TransitionDirection {
            TopToBottom,
            BottomToTop,
            LeftToRight,
            RightToLeft
        }

        public enum AnimationAxis {
            X,
            Y,
            Z
        }

        public enum CaseType {
            Car,
            House,
            Human
        }

        public enum GenderType {
            Unspecified,
            Female,
            Male
        }

        public enum StatusType {
            Unspecified,
            Available,
            Busy
        }
    }
}
