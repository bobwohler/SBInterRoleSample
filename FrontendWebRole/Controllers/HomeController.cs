using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FrontendWebRole.Models;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace FrontendWebRole.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // Simply redirect to Submit, since Submit
            // will serve as the front page of this application.
            return RedirectToAction("Submit");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }
        #region Submit Actions

        /// <summary>
        /// GET: /Home/Submit
        /// Controller method for for the submission form view.
        /// </summary>
        /// <returns>Submission Form View</returns>
        public ActionResult Submit()
        {
            // As usual, need to get a namespace manager for 
            // management operations.
            var namespaceManager = QueueConnector.CreateNamespaceManager();            

            // Get the queue and obtain the message count.
            var queue = namespaceManager.GetQueue(QueueConnector.QueueName);
            ViewBag.MessageCount = queue.MessageCount;
            return View();
        }

        /// <summary>
        /// POST: /Home/Submit
        /// Controller method for handling submissions from the submissions form.
        /// </summary>
        /// <returns>RedirectToAction</returns>
        [HttpPost]
        // Attribute to help prevent cross-site scripting attacks and cross-site request forgery.
        [ValidateAntiForgeryToken]
        public ActionResult Submit(OnlineOrder order)
        {
            // Validate model state and if good
            // submit to the SB queue.
            if (ModelState.IsValid)
            {
                // Create a brokered message for the incoming Order 
                // and submit it to the queue.
                // By default this constructor will use the
                // DataContract Serializer.
                var message = new BrokeredMessage(order);
                
                // Send it to the queue.
                QueueConnector.OrdersQueueClient.Send(message);
                
                return RedirectToAction("Submit");
            }
            else
            {
                return View(order);
            }
        }
        
        #endregion

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
