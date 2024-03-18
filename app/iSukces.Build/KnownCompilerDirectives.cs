using System.Diagnostics.CodeAnalysis;
namespace iSukces.Build;

[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
// ReSharper disable once UnusedType.Global
public static class KnownCompilerDirectives
{
    /// <summary>
    /// The event 'TrackWiresTools.GetInfoFromVisualElement' is never used
    /// </summary>
    public const string Cs0067_EventTrackWiresToolsGetInfoFromVisualElementIsNeverUsed = "0067";

    /// <summary>
    /// The given expression is never of the provided ('xxx') type
    /// </summary>
    public const string Cs0184_GivenExpressionIsNeverOfProvidedXxxType = "0184";

    /// <summary>
    /// The type or namespace name 'Xaml' does not exist in the namespace
    /// </summary>
    public const string Cs0234_TypeOrNamespaceNameXamlDoesNotExistInNamespace = "0234";

    /// <summary>
    /// There is no defined ordering between fields in multiple declarations of partial struct
    /// </summary>
    public const string Cs0282_ThereIsNoDefinedOrderingBetweenFieldsInMultipleDeclarationsOfPartialStruct = "0282";

    /// <summary>
    /// The class designer marked a member with the Obsolete attribute.
    /// </summary>
    public const string Cs0612_ClassDesignerMarkedMemberWithObsoleteAttribute = "0612";

    /// <summary>
    /// A class member was marked with the Obsolete attribute
    /// </summary>
    public const string Cs0618_ClassMemberWasMarkedWithObsoleteAttribute = "0618";

    /// <summary>
    /// Obsolete member 'BentPipe.PipeCircumferenceSegments' overrides non-obsolete member 'BentOrStraightPipe.PipeCircumferenceSegments'
    /// </summary>
    public const string Cs0809_ObsoleteMemberBentPipePipeCircumferenceSegmentsOverridesNonObsoleteMemberBentOrStraightPipePipeCircumferenceSegments = "0809";

    /// <summary>
    /// The default value specified for parameter 'startingKind' will have no effect because it applies to a member that is used in contexts that do not allow optional arguments
    /// </summary>
    public const string Cs1066_DefaultValueSpecifiedForParameterStartingKindWillHaveNoEffectBecauseItAppliesToMemberThatIsUsedInContextsThatDoNotAllowOptionalArguments = "1066";

    /// <summary>
    /// Empty switch block
    /// </summary>
    public const string Cs1522_EmptySwitchBlock = "1522";

    /// <summary>
    /// Missing XML comment for publicly visible type or member
    /// </summary>
    public const string Cs1591_MissingXmlCommentForPubliclyVisibleTypeOrMember = "1591";

    /// <summary>
    /// This async method lacks 'await'
    /// </summary>
    public const string Cs1998_AsyncMethodLacksAwait = "1998";

    /// <summary>
    /// 'WinNativeMethods.GetWindowStyle(nint)' does not need a CLSCompliant attribute because the assembly does not have a CLSCompliant attribute
    /// </summary>
    public const string Cs3021_WinNativeMethodsGetWindowStyleNintDoesNotNeedCLSCompliantAttributeBecauseAssemblyDoesNotHaveCLSCompliantAttribute = "3021";

    /// <summary>
    /// Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call
    /// </summary>
    public const string Cs4014_BecauseCallIsNotAwaitedExecutionOfCurrentMethodContinuesBeforeCallIsCompletedConsiderApplyingAwaitOperatorToResultOfCall = "4014";

    /// <summary>
    /// Thrown value may be null.
    /// </summary>
    public const string Cs8597_ThrownValueMayBeNull = "8597";

    /// <summary>
    /// Converting null literal or possible null value to non-nullable type.
    /// </summary>
    public const string Cs8600_ConvertingNullLiteralOrPossibleNullValueToNonNullableType = "8600";

    /// <summary>
    /// Possible null reference assignment.
    /// </summary>
    public const string Cs8601_PossibleNullReferenceAssignment = "8601";

    /// <summary>
    /// Dereference of a possibly null reference.
    /// </summary>
    public const string Cs8602_DereferenceOfPossiblyNullReference = "8602";

    /// <summary>
    /// Possible null reference return.
    /// </summary>
    public const string Cs8603_PossibleNullReferenceReturn = "8603";

    /// <summary>
    /// Possible null reference argument for parameter.
    /// </summary>
    public const string Cs8604_PossibleNullReferenceArgumentForParameter = "8604";

    /// <summary>
    /// Unboxing a possibly null value.
    /// </summary>
    public const string Cs8605_UnboxingPossiblyNullValue = "8605";

    /// <summary>
    /// A possible null value may not be used for a type marked with [NotNull] or [DisallowNull]
    /// </summary>
    public const string Cs8607_PossibleNullValueMayNotBeUsedForTypeMarkedWithNotNullOrDisallowNull = "8607";

    /// <summary>
    /// Nullability of reference types in type doesn't match overridden member.
    /// </summary>
    public const string Cs8608_NullabilityOfReferenceTypesInTypeDoesnTMatchOverriddenMember = "8608";

    /// <summary>
    /// Nullability of reference types in return type doesn't match overridden member.
    /// </summary>
    public const string Cs8609_NullabilityOfReferenceTypesInReturnTypeDoesnTMatchOverriddenMember = "8609";

    /// <summary>
    /// Nullability of reference types in type parameter doesn't match overridden member.
    /// </summary>
    public const string Cs8610_NullabilityOfReferenceTypesInTypeParameterDoesnTMatchOverriddenMember = "8610";

    /// <summary>
    /// Nullability of reference types in type parameter doesn't match partial method declaration.
    /// </summary>
    public const string Cs8611_NullabilityOfReferenceTypesInTypeParameterDoesnTMatchPartialMethodDeclaration = "8611";

    /// <summary>
    /// Nullability of reference types in type doesn't match implicitly implemented member.
    /// </summary>
    public const string Cs8612_NullabilityOfReferenceTypesInTypeOf = "8612";

    /// <summary>
    /// Nullability of reference types in return type doesn't match implicitly implemented member.
    /// </summary>
    public const string Cs8613_NullabilityOfReferenceTypesInReturnTypeDoesnTMatchImplicitlyImplementedMember = "8613";

    /// <summary>
    /// Nullability of reference types in type of parameter doesn't match implicitly implemented member.
    /// </summary>
    public const string Cs8614_NullabilityOfReferenceTypesInTypeOfParameterDoesnTMatchImplicitlyImplementedMember = "8614";

    /// <summary>
    /// Nullability of reference types in type doesn't match implemented member.
    /// </summary>
    public const string Cs8615_NullabilityOfReferenceTypesInTypeDoesnTMatchImplementedMember = "8615";

    /// <summary>
    /// Nullability of reference types in return type doesn't match implemented member.
    /// </summary>
    public const string Cs8616_NullabilityOfReferenceTypesInReturnTypeDoesnTMatchImplementedMember = "8616";

    /// <summary>
    /// Nullability of reference types in type of parameter doesn't match implemented member.
    /// </summary>
    public const string Cs8617_NullabilityOfReferenceTypesInTypeOfParameterDoesnTMatchImplementedMember = "8617";

    /// <summary>
    /// Non-nullable variable must contain a non-null value when exiting constructor. Consider declaring it as nullable.
    /// </summary>
    public const string Cs8618_NonNullablePropertyObjectsMustContainNonNullValueWhenExitingConstructor = "8618";

    /// <summary>
    /// Nullability of reference types in value doesn't match target type.
    /// </summary>
    public const string Cs8619_NullabilityOfReferenceTypesInValueOfType = "8619";

    /// <summary>
    /// Argument of type 'Func&lt;object, Task&gt;' cannot be used for parameter 'execute' of type 'Func&lt;object?, Task&gt;' in 'AsyncCommand.AsyncCommand(Func&lt;object?,
    /// </summary>
    public const string Cs8620_ArgumentCannotBeUsedForParameterDueToDifferencesInNullabilityOfReferenceTypes = "8620";

    /// <summary>
    /// Nullability of reference types in return type doesn't match the target delegate (possibly because of nullability attributes).
    /// </summary>
    public const string Cs8621_NullabilityOfReferenceTypesInReturnTypeOfLambdaExpression = "8621";

    /// <summary>
    /// Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
    /// </summary>
    public const string Cs8622_NullabilityOfReferenceTypesInTypeOfParameter = "8622";

    /// <summary>
    /// Argument cannot be used as an output due to differences in the nullability of reference types.
    /// </summary>
    public const string Cs8624_ArgumentCannotBeUsedAsAnOutputDueToDifferencesInNullabilityOfReferenceTypes = "8624";

    /// <summary>
    /// Cannot convert null literal to non-nullable reference type.
    /// </summary>
    public const string Cs8625_CannotConvertNullLiteralToNonNullableReferenceType = "8625";

    /// <summary>
    /// Nullable value type may be null.
    /// </summary>
    public const string Cs8629_NullableValueTypeMayBeNull = "8629";

    /// <summary>
    /// The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.
    /// </summary>
    public const string Cs8631_TypeCannotBeUsedAsTypeParameterInGenericTypeOrMethodNullabilityOfTypeArgumentDoesnTMatchConstraintType = "8631";

    /// <summary>
    /// The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
    /// </summary>
    public const string Cs8632_AnnotationForNullableReferenceTypesShouldOnlyBeUsedInCodeWithinNullableAnnotationsContext = "8632";

    /// <summary>
    /// Nullability in constraints for type parameter of method doesn't match the constraints for type parameter of interface method. Consider using an explicit interface implementation instead.
    /// </summary>
    public const string Cs8633_NullabilityInConstraintsForTypeParameterOfMethodDoesnTMatchConstraintsForTypeParameterOfInterfaceMethodConsiderUsingAnExplicitInterfaceImplementationInstead = "8633";

    /// <summary>
    /// The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
    /// </summary>
    public const string Cs8634_TypeSystemIODirectoryInfoCannotBeUsedAsTypeParameterTInGenericTypeOrMethod = "8634";

    /// <summary>
    /// Nullability of reference types in explicit interface specifier doesn't match interface implemented by the type.
    /// </summary>
    public const string Cs8643_NullabilityOfReferenceTypesInExplicitInterfaceSpecifierDoesnTMatchInterfaceImplementedByType = "8643";

    /// <summary>
    /// Type does not implement interface member. Nullability of reference types in interface implemented by the base type doesn't match.
    /// </summary>
    public const string Cs8644_TypeDoesNotImplementInterfaceMemberNullabilityOfReferenceTypesInInterfaceImplementedByBaseTypeDoesnTMatch = "8644";

    /// <summary>
    /// Member is already listed in the interface list on type with different nullability of reference types.
    /// </summary>
    public const string Cs8645_MemberIsAlreadyListedInInterfaceListOnTypeWithDifferentNullabilityOfReferenceTypes = "8645";

    /// <summary>
    /// The switch expression does not handle some null inputs (it is not exhaustive).
    /// </summary>
    public const string Cs8655_SwitchExpressionDoesNotHandleSomeNullInputsItIsNotExhaustive = "8655";

    /// <summary>
    /// Partial method declarations have inconsistent nullability in constraints for type parameter.
    /// </summary>
    public const string Cs8667_PartialMethodDeclarationsHaveInconsistentNullabilityInConstraintsForTypeParameter = "8667";

    /// <summary>
    /// Object or collection initializer implicitly dereferences possibly null member.
    /// </summary>
    public const string Cs8670_ObjectOrCollectionInitializerImplicitlyDereferencesPossiblyNullMember = "8670";

    /// <summary>
    /// The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
    /// </summary>
    public const string Cs8714_TypeTKeyCannotBeUsedAsTypeParameterTKeyInGenericTypeOrMethodDictionaryTKeyTValue = "8714";

    /// <summary>
    /// Parameter must have a non-null value when exiting.
    /// </summary>
    public const string Cs8762_ParameterMustHaveNonNullValueWhenExiting = "8762";

    /// <summary>
    /// A method marked [DoesNotReturn] should not return.
    /// </summary>
    public const string Cs8763_MethodMarkedDoesNotReturnShouldNotReturn = "8763";

    /// <summary>
    /// Nullability of return type doesn't match overridden member (possibly because of nullability attributes).
    /// </summary>
    public const string Cs8764_NullabilityOfReturnTypeDoesnTMatchOverriddenMemberPossiblyBecauseOfNullabilityAttributes = "8764";

    /// <summary>
    /// Nullability of type of parameter doesn't match overridden member (possibly because of nullability attributes).
    /// </summary>
    public const string Cs8765_NullabilityOfTypeOfParameterDoesnTMatchOverriddenMemberPossiblyBecauseOfNullabilityAttributes = "8765";

    /// <summary>
    /// Nullability of reference types in return type of doesn't match implicitly implemented member (possibly because of nullability attributes).
    /// </summary>
    public const string Cs8766_NullabilityOfReferenceTypesInReturnTypeOfDoesnTMatchImplicitlyImplementedMemberPossiblyBecauseOfNullabilityAttributes = "8766";

    /// <summary>
    /// Nullability of reference types in type of parameter of doesn't match implicitly implemented member (possibly because of nullability attributes).
    /// </summary>
    public const string Cs8767_NullabilityOfReferenceTypesInTypeOfParameterOfDoesnTMatchImplicitlyImplementedMemberPossiblyBecauseOfNullabilityAttributes = "8767";

    /// <summary>
    /// Nullability of reference types in return type doesn't match implemented member (possibly because of nullability attributes).
    /// </summary>
    public const string Cs8768_NullabilityOfReferenceTypesInReturnTypeDoesnTMatchImplementedMemberPossiblyBecauseOfNullabilityAttributes = "8768";

    /// <summary>
    /// Nullability of reference types in type of parameter doesn't match implemented member (possibly because of nullability attributes).
    /// </summary>
    public const string Cs8769_NullabilityOfReferenceTypesInTypeOfParameterDoesnTMatchImplementedMemberPossiblyBecauseOfNullabilityAttributes = "8769";

    /// <summary>
    /// Method lacks [DoesNotReturn] annotation to match implemented or overridden member.
    /// </summary>
    public const string Cs8770_MethodLacksDoesNotReturnAnnotationToMatchImplementedOrOverriddenMember = "8770";

    /// <summary>
    /// Member must have a non-null value when exiting.
    /// </summary>
    public const string Cs8774_MemberMustHaveNonNullValueWhenExiting = "8774";

    /// <summary>
    /// Member must have a non-null value when exiting.
    /// </summary>
    public const string Cs8775_MemberMustHaveNonNullValueWhenExiting = "8775";

    /// <summary>
    /// Member cannot be used in this attribute.
    /// </summary>
    public const string Cs8776_MemberCannotBeUsedInAttribute = "8776";

    /// <summary>
    /// Parameter must have a non-null value when exiting.
    /// </summary>
    public const string Cs8777_ParameterMustHaveNonNullValueWhenExiting = "8777";

    /// <summary>
    /// Nullability of reference types in return type doesn't match partial method declaration.
    /// </summary>
    public const string Cs8819_NullabilityOfReferenceTypesInReturnTypeDoesnTMatchPartialMethodDeclaration = "8819";

    /// <summary>
    /// Parameter must have a non-null value when exiting because parameter is non-null.
    /// </summary>
    public const string Cs8824_ParameterMustHaveNonNullValueWhenExitingBecauseParameterIsNonNull = "8824";

    /// <summary>
    /// Return value must be non-null because parameter is non-null.
    /// </summary>
    public const string Cs8825_ReturnValueMustBeNonNullBecauseParameterIsNonNull = "8825";

    /// <summary>
    /// The switch expression does not handle some null inputs (it is not exhaustive). However, a pattern with a 'when' clause might successfully match this value.
    /// </summary>
    public const string Cs8847_SwitchExpressionDoesNotHandleSomeNullInputsItIsNotExhaustiveHoweverPatternWithWhenClauseMightSuccessfullyMatchValue = "8847";

    /// <summary>
    /// This call site is reachable on all platforms
    /// </summary>
    public const string CA1416_CallSiteIsReachableOnAllPlatforms = "CA1416";

    /// <summary>
    /// Package was restored using old Framework This package may not be fully compatible with your project.
    /// </summary>
    public const string NU1701_PackageWasRestoredUsingOldFrameworkPackageMayNotBeFullyCompatibleWithYourProject = "NU1701";

}

