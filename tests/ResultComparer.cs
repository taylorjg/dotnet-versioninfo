using DotNetVersionInfo;
using System.Collections.Generic;

namespace Tests
{
    class ResultComparer : IEqualityComparer<Result>
    {
        public bool Equals(Result r1, Result r2)
        {
            var sr1 = r1 as SuccessResult;
            var sr2 = r2 as SuccessResult;
            if (sr1 != null && sr2 != null) {
                return
                    sr1.FileName == sr2.FileName &&
                    sr1.FileVersion == sr2.FileVersion &&
                    sr1.ProductVersion == sr2.ProductVersion;
            }

            var fr1 = r1 as FailureResult;
            var fr2 = r2 as FailureResult;
            if (fr1 != null && fr2 != null) {
                return
                    fr1.FileName == fr2.FileName &&
                    fr1.Error == fr2.Error;
            }

            return false;
        }

        public int GetHashCode(Result r)
        {
            return r.GetHashCode();
        }
    }
}
