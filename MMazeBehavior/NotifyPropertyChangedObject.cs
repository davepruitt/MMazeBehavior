using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMazeBehavior
{
    /// <summary>
    /// A simple class that all other classes can inherit from if they need to use the
    /// INotifyPropertyChanged notification system
    /// </summary>
    public class NotifyPropertyChangedObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged (string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region Methods to react to property changes on objects we are listening to

        protected virtual void ExecuteReactionsToModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //Get the name of the property that was changed on the child object
            string model_property_changed = e.PropertyName;

            //Get a System.Type object representing the current object that is reacting to changes on another object
            System.Type t = this.GetType();

            //Retrieve all property info for the view-model
            var property_info = t.GetProperties();

            //Iterate through each property
            foreach (var property in property_info)
            {
                //Get the custom attributes defined for this property
                var attributes = property.GetCustomAttributes(false);
                foreach (var attribute in attributes)
                {
                    //If the property is listening for changes on the model
                    var a = attribute as ReactToModelPropertyChanged;
                    if (a != null)
                    {
                        //If the property that was changed on the model matches the name
                        //that this view-model property is listening for...
                        if (a.ModelPropertyNames.Contains(model_property_changed))
                        {
                            //Notify the UI that the view-model property has been changed
                            NotifyPropertyChanged(property.Name);
                        }
                    }
                }
            }
        }

        #endregion
    }


}
