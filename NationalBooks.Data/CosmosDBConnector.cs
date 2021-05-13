using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using NationalBooks.Data.Models;

namespace NationalBooks.Data
{
    public class CosmosDBConnector
    {
        // The Cosmos DB endpoint and access key
        private readonly string _endpointUri;
        private readonly string _accessKey;

        // The name of the database and collections
        private readonly string _databaseName;
        private readonly string _bookCollectionName;
        private readonly string _orderCollectionName;

        private static readonly string _bookPartitionKey = "/Name";
        private static readonly string _orderPartitionKey = "/Status";

        private readonly DocumentClient _client;

        public CosmosDBConnector(string endpointUri,
                                 string accessKey,
                                 string databaseName,
                                 string bookCollectionName,
                                 string orderCollectionName)
        {
            this._endpointUri = endpointUri;
            this._accessKey = accessKey;
            this._databaseName = databaseName;
            this._bookCollectionName = bookCollectionName;
            this._orderCollectionName = orderCollectionName;

            this._client = new DocumentClient(new Uri(this._endpointUri), this._accessKey);

            ResourceResponse<Database> database =
                this._client.CreateDatabaseIfNotExistsAsync(new Database { Id = this._databaseName }).Result;


            DocumentCollection orderCollection = new DocumentCollection
            {
                Id = this._orderCollectionName
            };
            orderCollection.PartitionKey.Paths.Add(_bookPartitionKey);

            this._client.CreateDocumentCollectionAsync(
                UriFactory.CreateDatabaseUri(this._databaseName), orderCollection);


            DocumentCollection bookCollection = new DocumentCollection
            {
                Id = this._bookCollectionName
            };
            bookCollection.PartitionKey.Paths.Add(_orderPartitionKey);

            this._client.CreateDocumentCollectionAsync(
                UriFactory.CreateDatabaseUri(this._databaseName), bookCollection);
        }

        /// <summary>
        /// Execute a SQL query that gets all books
        /// </summary>
        public List<Book> RetrieveAllBooks()
        {
            List<Book> books = new List<Book>();            

            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };

            // Run a simple query via LINQ. 
            // Cosmos DB indexes all properties, so queries can be completed efficiently and with low latency.
            IQueryable<Book> bookQuery = this._client.CreateDocumentQuery<Book>(
                UriFactory.CreateDocumentCollectionUri(
                    this._databaseName,
                    this._bookCollectionName), queryOptions);


            if (bookQuery.AsEnumerable().FirstOrDefault() == null)
            {
                InitializeBooks();

                bookQuery = this._client.CreateDocumentQuery<Book>(
                     UriFactory.CreateDocumentCollectionUri(
                         this._databaseName,
                         this._bookCollectionName), queryOptions);
            }

            // The query is executed synchronously here, 
            // But can also be executed asynchronously via the IDocumentQuery<T> interface
            books = bookQuery.ToList();

            return books;
        }

        /// <summary>
        /// Execute a SQL query that gets an book by id
        /// </summary>
        private Book RetrieveBookById(Guid bookId)
        {
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };

            //retrieve the book by Id
            Book book = this._client.CreateDocumentQuery<Book>(
                UriFactory.CreateDocumentCollectionUri(
                    this._databaseName,
                    this._bookCollectionName), queryOptions)
                    .Where(c => c.Id == bookId)
                    .AsEnumerable()
                    .FirstOrDefault();

            return book;
        }

        /// <summary>
        /// Populates Cosmos DB with books
        /// </summary>
        private void InitializeBooks()
        {
            //define the book objects
            Book bookIfTomorrowComes= new Book
            {
                Id = Guid.NewGuid(),
                ImageUrl = "https://raw.githubusercontent.com/najmabegum/images/master/IfTomorrowComes.jpg",
                Name = "If Tomorrow Comes",
                Price = 1.2
            };

            Book bookChasingTomorrow = new Book
            {
                Id = Guid.NewGuid(),
                ImageUrl = "https://raw.githubusercontent.com/najmabegum/images/master/ChasingTomorrow.jpeg",
                Name = "Chansing Tomorrow",
                Price = 1.0
            };

            Book bookTheSilentWidow = new Book
            {
                Id = Guid.NewGuid(),
                ImageUrl = "https://raw.githubusercontent.com/najmabegum/images/master/TheSilentWidow.jpg",
                Name = "The Silent Widow",
                Price = 0.9
            };

            Book bookThePhoenix = new Book
            {
                Id = Guid.NewGuid(),
                ImageUrl = "https://raw.githubusercontent.com/najmabegum/images/master/ThePhoenix.jpg",
                Name = "The Phoenix",
                Price = 0.9
            };

            //add books to Cosmos DB
            CreateDocument(this._bookCollectionName, bookIfTomorrowComes);
            CreateDocument(this._bookCollectionName, bookChasingTomorrow);
            CreateDocument(this._bookCollectionName, bookTheSilentWidow);
            CreateDocument(this._bookCollectionName, bookThePhoenix);
        }

        /// <summary>
        /// Creates a new order and adds a book to it
        /// </summary>
        /// <param name="bookId">The id of the book to add to the order</param>
        public void AddBookToOrder(Guid bookId)
        {
            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };

            // Run a simple query via LINQ. 
            // Cosmos DB indexes all properties, so queries can be completed efficiently and with low latency.
            IQueryable<Order> orderQuery = this._client.CreateDocumentQuery<Order>(
                UriFactory.CreateDocumentCollectionUri(
                    this._databaseName,
                    this._orderCollectionName), queryOptions)
                    .Where(o => o.Status == "New");

            Order currentOrder = orderQuery.AsEnumerable().FirstOrDefault();

            if (currentOrder != null)
            {
                //loop through the lines of the order
                //and check if the book that we want to add is
                //already in there
                var orderLineExists = false;
                foreach (OrderLine lines in currentOrder.OrderLines)
                {
                    if (lines.Book.Id == bookId)
                    {
                        lines.Quantity++;
                        orderLineExists = true;

                        currentOrder.Price += lines.Book.Price;
                    }
                }

                //if the book is new in this order
                if (!orderLineExists)
                {
                    //get the book
                    Book book = RetrieveBookById(bookId);

                    //add it to a new orderline
                    currentOrder.OrderLines.Add(new OrderLine
                    {
                        Book = book,
                        Quantity = 1
                    });

                    currentOrder.Price += book.Price;
                }

                this._client.ReplaceDocumentAsync(
                    UriFactory.CreateDocumentUri(
                    this._databaseName,
                    this._orderCollectionName,
                    currentOrder.Id.ToString()),
                    currentOrder);
            }
            else
            {
                //if there is no order with status new
                //create one
                currentOrder = new Order
                {
                    Id = Guid.NewGuid(),
                    Date = DateTimeOffset.Now,
                    Status = "New"
                };

                Book book = RetrieveBookById(bookId);

                currentOrder.OrderLines.Add(new OrderLine
                {
                    Book = book,
                    Quantity = 1
                });

                currentOrder.Price += book.Price;

                CreateDocument(this._orderCollectionName, currentOrder);
            }
        }

        /// <summary>
        /// Execute a SQL query that gets all orders
        /// </summary>
        public List<Order> RetrieveAllOrders()
        {
            List<Order> orders = new List<Order>();

            // Set some common query options
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };

            // Run a simple query via LINQ. 
            // Cosmos DB indexes all properties, so queries can be completed efficiently and with low latency.
            IQueryable<Order> orderQuery = this._client.CreateDocumentQuery<Order>(
                UriFactory.CreateDocumentCollectionUri(
                    this._databaseName,
                    this._orderCollectionName), queryOptions);

            // The query is executed synchronously here, 
            // But can also be executed asynchronously via the IDocumentQuery<T> interface
            orders = orderQuery.ToList();

            return orders;
        }

        /// <summary>
        /// Execute a SQL query that gets an order by id
        /// </summary>
        public Order RetrieveOrderById(Guid orderId)
        {
            FeedOptions queryOptions = new FeedOptions { MaxItemCount = -1, EnableCrossPartitionQuery = true };

            //retrieve the order by Id
            Order order = this._client.CreateDocumentQuery<Order>(
                UriFactory.CreateDocumentCollectionUri(
                    this._databaseName,
                    this._orderCollectionName), queryOptions)
                    .Where(c => c.Id == orderId)
                    .AsEnumerable()
                    .FirstOrDefault();

            return order;
        }

        /// <summary>
        /// Remove an order by id
        /// </summary>
        public void CancelOrder(Guid orderId)
        {
            Order order = RetrieveOrderById(orderId);

            if (order != null)
            {
                this._client.DeleteDocumentAsync(
                     UriFactory.CreateDocumentUri(
                         this._databaseName,
                         this._orderCollectionName,
                         order.Id.ToString()),
                     new RequestOptions() { PartitionKey = new PartitionKey(Undefined.Value) });
            }
        }

        /// <summary>
        /// Change the status of an order by id
        /// </summary>
        public void PlaceOrder(Guid orderId)
        {
            Order order = RetrieveOrderById(orderId);

            if (order != null)
            {
                order.Status = "Placed";

                this._client.ReplaceDocumentAsync(
                    UriFactory.CreateDocumentUri(
                    this._databaseName,
                    this._orderCollectionName,
                    order.Id.ToString()),
                    order);
            }
        }

        /// <summary>
        /// Creates a new document in Cosmos DB in the given collection
        /// </summary>
        /// <param name="collectionname">Name of the Cosmos DB collection</param>
        /// <param name="document">Object to store in Cosmos DB</param>
        private void CreateDocument(string collectionname, object document)
        {
            this._client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(this._databaseName, collectionname), document);
        }
    }
}
