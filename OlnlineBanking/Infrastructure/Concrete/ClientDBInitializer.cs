using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using OlnlineBanking.Models;

namespace OlnlineBanking.Infrastructure.Concrete
{
    public class ClientDbInitializer : DropCreateDatabaseAlways<ClientDbContext>
    {
        protected override void Seed(ClientDbContext db)
        {
            db.Clients.Add(new Client(){FirstName = "Name1",LastName = "Last1",ContractNumber = "00987765",DateOfBirth=new DateTime(1979,10,26),Deposit = true,Phone = "435678",Status = "Classic"});
            db.Clients.Add(new Client() {FirstName = "Name2", LastName = "Last2", ContractNumber = "00987765", DateOfBirth = new DateTime(1979, 10, 26), Deposit = true, Phone = "435678", Status = "Classic" });
            db.Clients.Add(new Client() {FirstName = "Name3", LastName = "Last3", ContractNumber = "00987765", DateOfBirth = new DateTime(1979, 10, 26), Deposit = false, Phone = "435678", Status = "Classic" });
            db.Clients.Add(new Client() { FirstName = "Name4", LastName = "Last1", ContractNumber = "00987765", DateOfBirth = new DateTime(1979, 10, 26), Deposit = true, Phone = "435678", Status = "Classic" });
            db.Clients.Add(new Client() { FirstName = "Name5", LastName = "Last2", ContractNumber = "00987765", DateOfBirth = new DateTime(1979, 10, 26), Deposit = true, Phone = "435678", Status = "Classic" });
            db.Clients.Add(new Client() { FirstName = "Name6", LastName = "Last3", ContractNumber = "00987765", DateOfBirth = new DateTime(1979, 10, 26), Deposit = false, Phone = "435678", Status = "Classic" });
            db.Clients.Add(new Client() { FirstName = "Name7", LastName = "Last1", ContractNumber = "00987765", DateOfBirth = new DateTime(1979, 10, 26), Deposit = true, Phone = "435678", Status = "Classic" });
            db.Clients.Add(new Client() { FirstName = "Name8", LastName = "Last2", ContractNumber = "00987765", DateOfBirth = new DateTime(1979, 10, 26), Deposit = true, Phone = "435678", Status = "Classic" });
            db.Clients.Add(new Client() { FirstName = "Name9", LastName = "Last3", ContractNumber = "00987765", DateOfBirth = new DateTime(1979, 10, 26), Deposit = false, Phone = "435678", Status = "Classic" });
            base.Seed(db);
        }
    }

}