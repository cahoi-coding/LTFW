//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LTFW.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Love
    {
        public int postId { get; set; }
        public int userId { get; set; }
        public Nullable<System.DateTime> createdAt { get; set; }
    
        public virtual Post Post { get; set; }
        public virtual User User { get; set; }
    }
}