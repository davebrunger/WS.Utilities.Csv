# WS.Utilities.Csv
Utilities to aid parsing of CSV files. It is available as a [Nuget package](https://www.nuget.org/packages/WS.Utilities.Csv).

## Usage

### Reading CSV data from a file
    var data = new FileEnumerable(filename);
    var tokens = new CsvTokens(data);
    var records = new CsvRecords(tokens);
    foreach (var record in records) {
      // Process the CSV record
      foreach (var column in record) {
        // Process each column
      }
    }

### Reading CSV data from a stream
    var data = new StreamEnumerable(stream);
    var tokens = new CsvTokens(data);
    var records = new CsvRecords(tokens);
    foreach (var record in records) {
      // Process the CSV record
      foreach (var column in record) {
        // Process each column
      }
    }

### Reading CSV data from a file as objects
    var data = new FileEnumerable(filename);
    var tokens = new CsvTokens(data);
    var records = new CsvRecords(tokens);
    var createSomethingOrOther = (rowNumber, columns) => new SomethingOrOther(columns[0], columns[1], columns[2]);
    var somethingsOrOther = new CsvData<SomethingOrOther>(records, createSomethingOrOther);
    foreach (var somethingOrOther in somethingsOrOther) {
      // Process the SomethingOrOther
    }
