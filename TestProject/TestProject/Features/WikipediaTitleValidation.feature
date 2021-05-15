Feature: Wikipedia Title Validation
	Simple validate title on Wikipedia

Scenario: Validate Google's Title
	Given I have navigated to the "Google" page on Wikipedia
	Then the title of the page should be "Google - Wikipedia"

Scenario: Validate Microsoft's Title
	Given I have navigated to the "Microsoft" page on Wikipedia
	Then the title of the page should be "Microsoft - Wikipedia"

Scenario: Validate Apple's Title
	Given I have navigated to the "Apple" page on Wikipedia
	Then the title of the page should be "Apple - Wikipedia"