namespace Domain;

public class Consts
{
    public const string USERS_TABLE = "Users";
    public const string DISCOUNTS_TABLE = "Discounts";
    public const string PRODUCTS_TABLE = "Products";
    public const string RELATIONS_TABLE = "Relations";
    
    public const string ID = "ID";
    public const string AGE = "AGE";
    public const string GENDER = "GENDER";
    public const string CITY = "CITY";
    public const string COUNTRY = "COUNTRY";
    public const string CREATED_AT = "CREATED_AT";
    
    public const string OWNER_ID = "OWNER_ID";
    public const string NAME = "NAME";
    public const string DESCRIPTION = "DESCRIPTION";
    public const string CATEGORY = "CATEGORY";
    public const string SUBCATEGORY = "SUB_CATEGORY";
    public const string PRICE =  "PRICE";
    public const string QUANTITY = "QUANTITY";
    
    public const string PRODUCTS =  "PRODUCTS";
    public const string PERCENTAGE = "PERCENTAGE";
    public const string EXPIRED_AT = "EXPIRED_AT";
}

public enum RelationTypes
{
    Seller = 1,
    Buyer = 2
}

public enum RelationStatuses
{
    Active = 1,
    Inactive = 2,
    InProgress = 3,
    Completed = 4,
    Canceled = 5
}