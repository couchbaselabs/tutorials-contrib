package com.couchbasece.beersampler;


import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.EditText;

import com.couchbasece.beersampler.utils.DatabaseManager;

import static android.content.Context.MODE_PRIVATE;


public class LoginFragment extends Fragment {

    public LoginFragment() {
        // Required empty public constructor
    }


    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_login, container, false);
    }

    @Override
    public void onViewCreated(View view, Bundle savedInstanceState) {
        super.onViewCreated(view, savedInstanceState);
        Button buttonGuest = (Button) getView().findViewById(R.id.btn_guest);
        Button buttonLogin = (Button) getView().findViewById(R.id.btn_login);

        EditText usernameInput = (EditText) getView().findViewById(R.id.et_username);
        EditText passwordInput = (EditText) getView().findViewById(R.id.et_password);

        buttonGuest.setOnClickListener(new View.OnClickListener() {
                                           @Override
                                           public void onClick(View v) {

                                               String user = "D3m0u53r";
                                               String passwd = "D3m0u53r";

                                               // Stores username and password on Shared Preferences
                                               SharedPreferences sp=getActivity().getSharedPreferences("Login", MODE_PRIVATE);
                                               SharedPreferences.Editor Ed=sp.edit();
                                               Ed.putString("username", user);
                                               Ed.putString("password", passwd);
                                               Ed.commit();

                                               Intent intent = new Intent(getActivity().getApplicationContext(), BrowseDataActivity.class);
                                               startActivity(intent);
                                           }
                                       });

        buttonLogin.setOnClickListener(new View.OnClickListener() {
                                           @Override
                                           public void onClick(View v) {

                                               String user = usernameInput.getText().toString();
                                               String passwd = passwordInput.getText().toString();

                                               // Stores username and password on Shared Preferences
                                               SharedPreferences sp=getActivity().getSharedPreferences("Login", MODE_PRIVATE);
                                               SharedPreferences.Editor Ed=sp.edit();
                                               Ed.putString("username", user);
                                               Ed.putString("password", passwd);
                                               Ed.commit();

                                               DatabaseManager dbMgr = DatabaseManager.getSharedInstance();
                                               dbMgr.initCouchbaseLite(getActivity().getApplicationContext());
                                               dbMgr.OpenDatabaseForUser(user);
                                               DatabaseManager.startPushAndPullReplicationForCurrentUser(user, passwd);

                                               Intent intent = new Intent(getActivity().getApplicationContext(), BrowseDataActivity.class);
                                               startActivity(intent);
                                           }
                                       });
    }


}
