/**
 * @license Copyright (c) 2003-2020, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see https://ckeditor.com/legal/ckeditor-oss-license
 */


CKEDITOR.editorConfig = function( config ) {
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
	// config.uiColor = '#AADC6E';

    // disable the content filter
    config.allowedContent = true;

    // upload url setting
	config.filebrowserUploadUrl = _uploadFilePath + "?";

    // upload request headers
    config.fileTools_requestHeaders = {
        'X-Requested-With': 'XMLHttpRequest',
        'X-CSRF-TOKEN': 'CSFR-Token'
    };
};
