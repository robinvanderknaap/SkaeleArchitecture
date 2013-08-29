using System;
using System.Web.Mvc;
using Infrastructure.FilterAttributes;
using Moq;
using NHibernate;
using NUnit.Framework;
using Tests.Utils.TestFixtures;

namespace Tests.Unit.Infrastructure.FilterAttributes
{
    class UnitOfWorkFilterTests : BaseTestFixture
    {
        [Test]
        public void TransactionIsStartedBeforeActionIsStarted()
        {
            var transactionMock = new Mock<ITransaction>();

            var sessionMock = new Mock<ISession>();
            sessionMock.Setup(x => x.Transaction).Returns(transactionMock.Object);

            new UnitOfWorkFilter(sessionMock.Object).OnActionExecuting(new Mock<ActionExecutingContext>().Object);

            transactionMock.Verify(x=>x.Begin(), Times.Once());
        }

        [Test]
        public void TransactionIsCommittedWhenActionIsFinishedAndTransactionIsActive()
        {
            var transactionMock = new Mock<ITransaction>();
            transactionMock.Setup(x => x.IsActive).Returns(true);

            var sessionMock = new Mock<ISession>();
            sessionMock.Setup(x => x.Transaction).Returns(transactionMock.Object);

            new UnitOfWorkFilter(sessionMock.Object).OnActionExecuted(new Mock<ActionExecutedContext>().Object);

            transactionMock.Verify(x => x.Commit(), Times.Once());
        }

        [Test]
        public void TransactionIsNotCommittedWhenActionIsFinishedAndTransactionIsNotActive()
        {
            var transactionMock = new Mock<ITransaction>();
            transactionMock.Setup(x => x.IsActive).Returns(false);

            var sessionMock = new Mock<ISession>();
            sessionMock.Setup(x => x.Transaction).Returns(transactionMock.Object);

            new UnitOfWorkFilter(sessionMock.Object).OnActionExecuted(new Mock<ActionExecutedContext>().Object);

            transactionMock.Verify(x => x.Commit(), Times.Never());
        }

        [Test]
        public void TransactionIsNotCommittedWhenActionIsFinishedAndTransactionIsNull()
        {
            var sessionMock = new Mock<ISession>();
            sessionMock.Setup(x => x.Transaction).Returns((ITransaction)null);

            Assert.DoesNotThrow(() => new UnitOfWorkFilter(sessionMock.Object).OnActionExecuted(new Mock<ActionExecutedContext>().Object));
        }

        [Test]
        public void TransactionIsRolledBackWhenExceptionOccured()
        {
            var filterContextMock = new Mock<ActionExecutedContext>();
            filterContextMock.Setup(x => x.Exception).Returns(new NullReferenceException());
            
            var transactionMock = new Mock<ITransaction>();
            transactionMock.Setup(x => x.IsActive).Returns(true);

            var sessionMock = new Mock<ISession>();
            sessionMock.Setup(x => x.Transaction).Returns(transactionMock.Object);

            new UnitOfWorkFilter(sessionMock.Object).OnActionExecuted(filterContextMock.Object);

            transactionMock.Verify(x => x.Commit(), Times.Never());
            transactionMock.Verify(x => x.Rollback(), Times.Once());
        }

    }
}
