-- Idempotent tenant script: form-template key + icon metadata on InsuranceTypeMaster.
-- Does NOT update PolicyDetails.InsuranceTypeID (historical FKs stay unchanged).
-- Run on every tenant database.

IF COL_LENGTH('dbo.InsuranceTypeMaster', 'InsuranceTypeCode') IS NULL
    ALTER TABLE dbo.InsuranceTypeMaster
        ADD InsuranceTypeCode NVARCHAR(64) NOT NULL
        CONSTRAINT DF_ITM_InsuranceTypeCode DEFAULT (N'');

IF COL_LENGTH('dbo.InsuranceTypeMaster', 'IconClass') IS NULL
    ALTER TABLE dbo.InsuranceTypeMaster
        ADD IconClass NVARCHAR(128) NOT NULL
        CONSTRAINT DF_ITM_IconClass DEFAULT (N'');
