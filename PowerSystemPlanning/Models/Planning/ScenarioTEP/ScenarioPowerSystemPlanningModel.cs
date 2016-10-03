using NLog;
using PowerSystemPlanning.Models.Planning.LDC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PowerSystemPlanning.Models.Planning.ScenarioTEP
{
    // TODO decoupled open & save (a collection of serializable objects which can be modified in runtime by plugins)
    // TODO extend bindableBase instead of customized INPC implementation
    /// <summary>
    /// Encapsulates data for power system planning under a small number of future scenarios.
    /// </summary>
    /// <remarks>
    /// Encapsulates a full power system data object for each scenario being modeled. This rather heavy object model is thought to be used for power system planning under 10 or less scenarios.
    /// </remarks>
    public class ScenarioPowerSystemPlanningModel : INotifyPropertyChanged
    {
        /// <summary>
        /// NLog Logger for this class.
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public event PropertyChangedEventHandler PropertyChanged;

        // Example implementation from: https://msdn.microsoft.com/en-us/library/system.componentmodel.inotifypropertychanged(v=vs.110).aspx
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        string _Name;
        /// <summary>
        /// The name of this test case
        /// </summary>
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    NotifyPropertyChanged();
                }
            }
        }
        /// <summary>
        /// The load duration curve that describes demand under this particular scenario
        /// </summary>
        public LoadDurationCurveByBlocks MyLoadDurationCurve { get; set; }
        /// <summary>
        /// The target planning year (i.e. the year in the future when the scenarios will be realized).
        /// </summary>
        public int TargetPlanningYear { get; set; }
        /// <summary>
        /// The number of years that the operation in this scenarios will be valid (starting from <see cref="TargetPlanningYear"/>)
        /// </summary>
        public int YearsWithOperation { get; set; }
        /// <summary>
        /// The future scenarios being modeled (each scenario is a full power system data object).
        /// </summary>
        public BindingList<PowerSystemScenario> MyScenarios { get; set; }
        // TODO define a basis power system from which scenarios will derive (shared Nodes, Generator ID/Name, etc)

        double _YearlyDiscountRate;
        /// <summary>
        /// The rate used to discount future cash flows (often below 10%).
        /// </summary>
        public double YearlyDiscountRate
        {
            get
            {
                return _YearlyDiscountRate;
            }
            set
            {
                if (_YearlyDiscountRate != value)
                {
                    _YearlyDiscountRate = value;
                    NotifyPropertyChanged();
                }
            }
        }
        /// <summary>
        /// The factor used to discount future cash flows.
        /// </summary>
        /// <remarks>
        /// Equals \f$ \frac{1}{1+DiscountRate}\f$. Hence, a cash flow of \f$ C \f$ in year \f$ N \f$ has a present value of 
        /// \f$ C \cdot YearlyDiscountFactor^N \f$.
        /// </remarks>
        public double YearlyDiscountFactor { get { return 1 / (1 + YearlyDiscountRate); } }

        /// <summary>
        /// The factor used to transform operation costs in the target planning year, into present value.
        /// </summary>
        /// <remarks>
        /// It is assumed that operation costs are calculated for the <see cref="TargetPlanningYear"/>=N, 
        /// and these costs remain unchanged for <see cref="YearsWithOperation"/>=M more years. 
        /// The costs incurred in year N are considered to be a cash-flow received at the beginning of year N,
        /// so these costs are discounted by \f$ \frac{1}{(1+r)^N} \f$.
        /// 
        /// The value of the costs incurred between years \f$ N+1 \f$ and \f$ N+M \f$, in year \f$ N \f$, is:
        /// \f[
        ///     \sum_{t=1}^{M} \frac{1}{(1+r)^t} = \frac{1-(1+r)^{-(M-1)}}{r}
        /// \f]
        /// Note that costs incurred in year M are accounted for at the beginning of year M. 
        /// So as not to count the year 0, these costs must be discounted \f$ M-1 \f$ years.
        /// 
        /// That value (in \f$ t=N \f$) is brought to present value along with the costs incurred in year \f$ N \f$, 
        /// yielding the following factor used to transform yearly operation costs into present value:
        /// \f[
        ///     \frac{1}{(1+r)^N} \cdot \left( 1 + \frac{1-(1+r)^{-(M-1)}}{r} \right)
        /// \f]
        /// </remarks>
        public double OperationPresentValueFactor
        {
            get
            {
                var d = YearlyDiscountFactor;
                var p1 = 1 + (1 - Math.Pow(d, YearsWithOperation - 1)) / YearlyDiscountRate;
                var factor = Math.Pow(d, TargetPlanningYear) * p1;
                return factor;
            }
        }

        private string _FullFileName;
        /// <summary>
        /// The full file name (including path) of the target XML file for saving this power system model.
        /// </summary>
        public string FullFileName
        {
            get
            {
                return _FullFileName;
            }

            set
            {
                if (this._FullFileName != value)
                {
                    this._FullFileName = value;
                    NotifyPropertyChanged();
                }
            }
        }
        /// <summary>
        /// The XMLSerializer which will be used to save this object (must be overriden by deriving classes).
        /// </summary>
        public virtual XmlSerializer MyXmlSerializer { get { return new XmlSerializer(typeof(ScenarioPowerSystemPlanningModel)); } }

        /// <summary>
        /// Indicates whether the target XML file for saving this power system model already exists.
        /// </summary>
        public bool IsSaved { get { return File.Exists(this.FullFileName); } }

        public ScenarioPowerSystemPlanningModel()
        {
            Name = "";
            TargetPlanningYear = 0;
            YearlyDiscountRate = 0;
            MyLoadDurationCurve = new LoadDurationCurveByBlocks();
            MyScenarios = new BindingList<PowerSystemScenario>();
        }

        public ScenarioPowerSystemPlanningModel(string name, double yearlyDiscountRate, LoadDurationCurveByBlocks myLoadDurationCurve) : this()
        {
            Name = name;
            YearlyDiscountRate = yearlyDiscountRate;
            MyLoadDurationCurve = myLoadDurationCurve;
        }

        /// <summary>
        /// Saves this power system model to the (previously-set) path in <see cref="FullFileName"/>.
        /// </summary>
        public void saveToXMLFile()
        {
            using (TextWriter saveStream = new StreamWriter(FullFileName))
            {
                // Serialize XML document
                MyXmlSerializer.Serialize(saveStream, this);
            }
            logger.Info("Current model (named '{0}') saved in {1}.", Name, FullFileName);
        }
        /// <summary>
        /// Saves this power system model to the file in the path provided as argument.
        /// </summary>
        /// <param name="file_name">The full-path to which this model will be saved.</param>
        public void saveToXMLFile(string file_name)
        {
            FullFileName = file_name;
            saveToXMLFile();
        }
    }
}
