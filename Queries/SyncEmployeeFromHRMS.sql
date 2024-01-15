-- clear old data
TRUNCATE TABLE DSG_Employee;

-- insert new data
INSERT INTO DSG_Employee(
  EmployeeId,
  [Name],
  LastName,
  NameEng,
  Company,
  PositionCode,
  PositionName,
  DivisionCode,
  DivisionName,
  DepartmentCode,
  DepartmentName,
  [Status],
  EmployeeIdOld,
  Email
)
SELECT 
  E.CODEMPID,
  E.EMPNAME,
  E.EMPLASTNAME,
  E.NAMEENG,
  E.COMPANYNAME,
  E.POSITIONCODE,
  E.POSITIONNAME,
  E.DIVISIONCODE,
  E.DIVISIONNAME,
  E.DEPARTMENTCODE,
  E.DEPARTMENTNAME,
  E.STATUS,
  E.OCODEMPID,
  T.EMAIL
FROM [HRTRAINING].[dbo].EMPLOYEE E
LEFT JOIN [HRTRAINING].[dbo].TEMPLOY1 T 
  ON T.CODEMPID = E.CODEMPID
WHERE E.STATUS = 3
AND E.EMPLASTNAME IS NOT NULL;