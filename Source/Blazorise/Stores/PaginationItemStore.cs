﻿#region Using directives
using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
#endregion

namespace Blazorise.Stores
{
    public struct PaginationItemStore : IEquatable<PaginationItemStore>
    {
        #region Methods

        public override bool Equals( object obj )
            => obj is PaginationItemStore store && Equals( store );

        public bool Equals( PaginationItemStore other )
        {
            return Active == other.Active
                && Disabled == other.Disabled;
        }

        public override int GetHashCode()
        {
            // Use a different bit for bool fields: bool.GetHashCode() will return 0 (false) or 1 (true). So we would
            // end up having the same hash code for e.g. two instances where one has only noCache set and the other
            // only noStore.
            var result = Active.GetHashCode()
             ^ ( Disabled.GetHashCode() << 1 ); // increase shift by one for every bool field

            return result;
        }

        public static bool operator ==( PaginationItemStore lhs, PaginationItemStore rhs )
        {
            return lhs.Equals( rhs );
        }

        public static bool operator !=( PaginationItemStore lhs, PaginationItemStore rhs )
        {
            return !lhs.Equals( rhs );
        }

        #endregion

        #region Properties

        public bool Active { readonly get; set; }

        public bool Disabled { readonly get; set; }

        #endregion
    }
}
