﻿/*
 * HFM.NET - Protein Class Tests
 * Copyright (C) 2009-2013 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using NUnit.Framework;

using HFM.Core.DataTypes;

namespace HFM.Proteins.Tests
{
   [TestFixture]
   public class ProteinTests
   {
      [Test]
      public void Protein_DefaultValues_Test()
      {
         var protein = new Protein();
         Assert.AreEqual(0, protein.ProjectNumber);
         Assert.AreEqual("0.0.0.0", protein.ServerIP);
         Assert.AreEqual("Unknown", protein.WorkUnitName);
         Assert.AreEqual(0, protein.NumberOfAtoms);
         Assert.AreEqual(0, protein.PreferredDays);
         Assert.AreEqual(0, protein.MaximumDays);
         Assert.AreEqual(0, protein.Credit);
         Assert.AreEqual(100, protein.Frames);
         Assert.AreEqual("Unknown", protein.Core);
         Assert.AreEqual("Unassigned Description", protein.Description);
         Assert.AreEqual("Unknown", protein.Contact);
         Assert.AreEqual(0, protein.KFactor);
      }

      [Test]
      public void Protein_IsUnknown_Test1()
      {
         var protein = new Protein { ProjectNumber = 1 };
         Assert.IsFalse(protein.IsUnknown());
      }

      [Test]
      public void Protein_IsUnknown_Test2()
      {
         var protein = new Protein();
         Assert.IsTrue(protein.IsUnknown());
      }

      [Test]
      public void Protein_IsUnknown_Test3()
      {
         Protein protein = null;
         Assert.IsTrue(protein.IsUnknown());
      }

      [Test]
      public void Protein_IsValid_Test1()
      {
         var protein = new Protein { ProjectNumber = 1, PreferredDays = 3, MaximumDays = 5, Credit = 500, Frames = 100, KFactor = 26.4 };
         Assert.IsTrue(protein.IsValid());
      }

      [Test]
      public void Protein_IsValid_Test2()
      {
         var protein = new Protein();
         Assert.IsFalse(protein.IsValid());
      }

      [Test]
      public void Protein_IsValid_Test3()
      {
         Protein protein = null;
         Assert.IsFalse(protein.IsValid());
      }
   }
}
