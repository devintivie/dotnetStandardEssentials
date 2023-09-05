using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetStandardEssentials.Results
{
    public class ResultCollection : IEnumerable<IResult>
    {
        #region Fields

        #endregion

        #region Properties
        public List<IResult> Results { get; } = new List<IResult>();
        
        public bool HasErrors => Results.Any();

        #endregion

        #region Constructors
        public ResultCollection()
        {
            
        }

        public ResultCollection(IEnumerable<Result> results)
        {
            foreach (Result result in results)
            {
                AddResultIfError(result);
            }    
        }

        #endregion

        #region Methods
        public void AddResultIfError(Result result)
        {
            if (result.Exception is null)
            {
                return;
            }

            Results.Add(result);
        }

        public void AddException(Exception exception)
        {
            Result result = new Result(exception);
            Results.Add(result);
        }

        public void AddResultCollection(ResultCollection collection)
        {
            if (!collection.Any())
            {
                return;
            }

            Results.AddRange(collection);
        }
        #endregion

        #region IEnumerable Methods
        public IEnumerator<IResult> GetEnumerator()
        {
            return Results.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        
    }
}
